namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;

    /// <summary>
    /// Provides information about the environment.
    /// </summary>
    internal class EnvironmentInfoProvider : IEnvironmentInfoProvider, IAppLoadInitializable
    {
        private readonly ILogger<EnvironmentInfoProvider> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentInfoProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public EnvironmentInfoProvider(ILogger<EnvironmentInfoProvider> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public event EventHandler OnReady;

        /// <inheritdoc />
        public IDictionary<string, Version> ControlVersions { get; private set; }

        /// <inheritdoc />
        public Version PlatformVersion { get; private set; }

        /// <inheritdoc/>
        public bool IsReady { get; private set; }

        /// <inheritdoc />
        public async Task InitializeAsync(IPage page)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            this.ControlVersions = await GetControlVersionsAsync(page);
            this.PlatformVersion = await GetPlatformVersionAsync(page);

            this.IsReady = true;
            this.OnReady?.Invoke(this, EventArgs.Empty);
        }

        private static async Task<IDictionary<string, Version>> GetControlVersionsAsync(IPage page)
        {
            var customControlsResponse = await page.APIRequest.GetAsync($"https://{new Uri(page.Url).Host}/api/data/v9.2/customcontrols?$select=name,version");
            var customControlsJson = await customControlsResponse.JsonAsync();

            if (!customControlsResponse.Ok)
            {
                throw new PowerPlaywrightException($"Unable to retrieve custom controls from environment. Status code: {customControlsResponse.Status}.");
            }

            return customControlsJson?.GetProperty("value").EnumerateArray().ToDictionary(
                c => c.GetProperty("name").GetString(),
                c => new Version(c.GetProperty("version").GetString()));
        }

        private static async Task<Version> GetPlatformVersionAsync(IPage page)
        {
            return new Version(await page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().getVersion()"));
        }
    }
}