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
    public class RedirectionInfoProvider : IRedirectionInfoProvider, IAppLoadInitializable
    {
        private static readonly string[] AppSettingKeys = new[] { "NewLookAlwaysOn", "NewLookOptOut", "AppChannel" };

        private readonly JsonSerializerOptions serializerOptions;

        private RedirectionInfo redirectionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectionInfoProvider"/> class.
        /// </summary>
        public RedirectionInfoProvider()
        {
            this.serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        /// <inheritdoc/>
        public IRedirectionInfo GetRedirectionInfo()
        {
            return this.redirectionInfo;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync(IPage page)
        {
            this.redirectionInfo = new RedirectionInfo(
                await this.GetVersionAsync(page),
                await this.GetOrgSettingsAsync(page),
                await this.GetAppSettingsAsync(page),
                await this.GetUserSettingsAsync(page));
        }

        private async Task<Version> GetVersionAsync(IPage page)
        {
            return new Version(await page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().getVersion()"));
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
                @"(keys) => {
                    const globalContext = Xrm.Utility.getGlobalContext();
                    const appSettings = {};
                    keys.forEach(key => {
                        appSettings[key] = globalContext.getCurrentAppSetting(key);
                    });
                    return appSettings;
                }",
                AppSettingKeys);

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