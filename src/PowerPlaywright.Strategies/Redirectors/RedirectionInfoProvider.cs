namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Provides the information required for control redirection.
    /// </summary>
    public class RedirectionInfoProvider : IRedirectionInfoProvider<RedirectionInfo>, IAppLoadInitializable
    {
        private readonly JsonSerializerOptions serializerOptions;

        private RedirectionInfo redirectionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectionInfoProvider"/> class.
        /// </summary>
        public RedirectionInfoProvider()
        {
            this.serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <inheritdoc/>
        public RedirectionInfo GetRedirectionInfo()
        {
            return this.redirectionInfo;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync(IPage page)
        {
            this.redirectionInfo = new RedirectionInfo(
                await this.GetOrgSettingsAsync(page),
                await this.GetAppSettingsAsync(page),
                await this.GetUserSettingsAsync(page));
        }

        private async Task<OrgSettings> GetOrgSettingsAsync(IPage page)
        {
            var orgSettings = await page.EvaluateAsync(
                @"async () => {
                   var response = await Xrm.WebApi.online.retrieveMultipleRecords('organization', '?$select=releasechannel');
                   return response.entities[0];
                }");

            if (!orgSettings.HasValue)
            {
                throw new PowerPlaywrightException("Unable to retrieve app settings");
            }

            return orgSettings.Value.Deserialize<OrgSettings>();
        }

        private async Task<AppSettings> GetAppSettingsAsync(IPage page)
        {
            var appSettings = await page.EvaluateAsync(
                @"() => {
                    var appSettings = Xrm.Utility.getGlobalContext().getCurrentAppSettings();
                    return appSettings;
                }");

            if (!appSettings.HasValue)
            {
                throw new PowerPlaywrightException("Unable to retrieve app settings");
            }

            return appSettings.Value.Deserialize<AppSettings>();
        }

        private async Task<UserSettings> GetUserSettingsAsync(IPage page)
        {
            var appId = await page.EvaluateAsync<Guid>("async () => { " +
                "   var properties = await Xrm.Utility.getGlobalContext().getCurrentAppProperties();" +
                "   return properties.appId" +
                "}");
            var userId = await page.EvaluateAsync<Guid>("Xrm.Utility.getGlobalContext().userSettings.userId");
            var userSettings = await page.EvaluateAsync("async (userId) => Xrm.WebApi.online.retrieveRecord('usersettings', userId, '?$select=trytogglesets,releasechannel')", userId);

            if (!userSettings.HasValue)
            {
                throw new PowerPlaywrightException("Unable to retrieve user settings");
            }

            if (this.serializerOptions.Converters.Count == 0)
            {
                this.serializerOptions.Converters.Add(new AppTogglesConverter(appId));
            }

            return JsonSerializer.Deserialize<UserSettings>(userSettings.Value, this.serializerOptions);
        }
    }
}