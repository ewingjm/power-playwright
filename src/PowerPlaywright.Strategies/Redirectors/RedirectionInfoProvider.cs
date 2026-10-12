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

        private static readonly int ApiAvailabilityTimeoutMs = 30000;

        private static readonly string VersionApiCondition =
            "typeof Xrm !== 'undefined' && typeof Xrm.Utility?.getGlobalContext === 'function'";

        private static readonly string OrgSettingsApiCondition =
            "typeof Xrm !== 'undefined' && typeof Xrm.WebApi?.online?.retrieveMultipleRecords === 'function'";

        private static readonly string AppSettingsApiCondition =
            "typeof Xrm !== 'undefined' && typeof Xrm.Utility?.getGlobalContext === 'function' && typeof Xrm.Utility.getGlobalContext().getCurrentAppSetting === 'function'";

        private static readonly string UserSettingsApiCondition =
            "typeof Xrm !== 'undefined' && typeof Xrm.Utility?.getGlobalContext === 'function' && " +
            "typeof Xrm.Utility.getGlobalContext().getCurrentAppProperties === 'function' && " +
            "typeof Xrm.Utility.getGlobalContext().userSettings?.userId !== 'undefined' && " +
            "typeof Xrm.WebApi?.online?.retrieveRecord === 'function'";

        private readonly JsonSerializerOptions serializerOptions;

        private RedirectionInfo environmentInfo;

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

        /// <summary>
        /// Waits for the given client API condition to become truthy before returning, so that callers don't
        /// attempt to invoke client APIs before they've been defined on the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="condition">A JavaScript expression that evaluates to a truthy value once the required API(s) are available.</param>
        /// <param name="apiName">A human-readable name for the API(s) being waited for, used in the exception message if the wait times out.</param>
        private static async Task WaitForApiAsync(IPage page, string condition, string apiName)
        {
            try
            {
                await page.WaitForFunctionAsync(condition, options: new PageWaitForFunctionOptions { Timeout = ApiAvailabilityTimeoutMs });
            }
            catch (TimeoutException ex)
            {
                throw new PowerPlaywrightException($"Timed out after {ApiAvailabilityTimeoutMs}ms waiting for the '{apiName}' client API to become available.", ex);
            }
        }

        /// <inheritdoc/>
        public IRedirectionEnvironmentInfo GetRedirectionInfo()
        {
            return this.environmentInfo;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync(IPage page)
        {
            this.environmentInfo = new RedirectionInfo(
                await this.GetVersionAsync(page),
                await this.GetOrgSettingsAsync(page),
                await this.GetAppSettingsAsync(page),
                await this.GetUserSettingsAsync(page));
        }

        private async Task<Version> GetVersionAsync(IPage page)
        {
            await WaitForApiAsync(page, VersionApiCondition, "Xrm.Utility.getGlobalContext");

            return new Version(await page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().getVersion()"));
        }

        private async Task<OrgSettings> GetOrgSettingsAsync(IPage page)
        {
            await WaitForApiAsync(page, OrgSettingsApiCondition, "Xrm.WebApi.online.retrieveMultipleRecords");

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
            await WaitForApiAsync(page, AppSettingsApiCondition, "Xrm.Utility.getGlobalContext().getCurrentAppSetting");

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
            await WaitForApiAsync(
                page,
                "typeof Xrm !== 'undefined' && typeof Xrm.Utility?.getGlobalContext === 'function' && " +
                "typeof Xrm.Utility.getGlobalContext().getCurrentAppProperties === 'function' && " +
                "typeof Xrm.Utility.getGlobalContext().userSettings?.userId !== 'undefined' && " +
                "typeof Xrm.WebApi?.online?.retrieveRecord === 'function'",
                "Xrm.Utility.getGlobalContext().getCurrentAppProperties/userSettings/Xrm.WebApi.online.retrieveRecord");

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