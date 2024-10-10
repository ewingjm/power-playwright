namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Provides the information required for control redirection.
    /// </summary>
    public class RedirectionInfoProvider : IRedirectionInfoProvider<RedirectionInfo>, IAppLoadInitializable
    {
        private RedirectionInfo redirectionInfo;

        /// <inheritdoc/>
        public RedirectionInfo GetRedirectionInfo()
        {
            return this.redirectionInfo;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync(IPage page)
        {
            var appId = await page.EvaluateAsync<Guid>("async () => { " +
                "   var properties = await Xrm.Utility.getGlobalContext().getCurrentAppProperties();" +
                "   return properties.appId" +
                "}");
            var userId = await page.EvaluateAsync<Guid>("Xrm.Utility.getGlobalContext().userSettings.userId");
            var userSettings = await page.EvaluateAsync("async (userId) => Xrm.WebApi.online.retrieveRecord('usersettings', userId, '?$select=trytogglesets')", userId);
            var toggleSetsString = userSettings.Value.GetProperty("trytogglesets").GetString();

            if (!string.IsNullOrEmpty(toggleSetsString) && JsonNode.Parse(toggleSetsString).AsObject().TryGetPropertyValue(appId.ToString(), out var appTogglesJson))
            {
                this.redirectionInfo = new RedirectionInfo(appTogglesJson.Deserialize<AppToggles>());
                return;
            }

            this.redirectionInfo = new RedirectionInfo(null);
        }
    }
}