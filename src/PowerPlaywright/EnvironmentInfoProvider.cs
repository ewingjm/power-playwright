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
        private static readonly Dictionary<string, EnvironmentInfo> EnvironmentInfoCache = new Dictionary<string, EnvironmentInfo>();

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
        public Version PlatformVersion { get; private set; }

        /// <inheritdoc />
        public IDictionary<string, Version> ControlVersions { get; private set; }

        /// <inheritdoc />
        public IDictionary<string, Guid> ControlIds { get; private set; }

        /// <inheritdoc/>
        public bool IsReady { get; private set; }

        /// <inheritdoc />
        public async Task InitializeAsync(IPage page)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var environmentUrl = new Uri(page.Url).GetLeftPart(UriPartial.Authority);

            EnvironmentInfo environmentInfo;
            if (EnvironmentInfoCache.TryGetValue(environmentUrl, out var cachedInfo))
            {
                this.logger.LogInformation($"Using cached environment info for {environmentUrl}");
                environmentInfo = cachedInfo;
            }
            else
            {
                var controls = await GetControlsAsync(page);
                var platformVersion = await GetPlatformVersionAsync(page);

                environmentInfo = new EnvironmentInfo
                {
                    ControlIds = controls.ToDictionary(c => c.Name, c => c.Id),
                    ControlVersions = controls.ToDictionary(c => c.Name, c => c.Version),
                    PlatformVersion = platformVersion,
                };

                EnvironmentInfoCache[environmentUrl] = environmentInfo;
                this.logger.LogInformation($"Cached environment info for {environmentUrl}");
            }

            this.ControlIds = environmentInfo.ControlIds;
            this.ControlVersions = environmentInfo.ControlVersions;
            this.PlatformVersion = environmentInfo.PlatformVersion;

            this.IsReady = true;
            this.OnReady?.Invoke(this, EventArgs.Empty);
        }

        private static async Task<IEnumerable<Control>> GetControlsAsync(IPage page)
        {
            var customControlsResponse = await page.APIRequest.GetAsync($"https://{new Uri(page.Url).Host}/api/data/v9.2/customcontrols?$select=name,version,customcontrolid");
            var customControlsJson = await customControlsResponse.JsonAsync();

            if (!customControlsResponse.Ok)
            {
                throw new PowerPlaywrightException($"Unable to retrieve custom controls from environment. Status code: {customControlsResponse.Status}.");
            }

            return customControlsJson?.GetProperty("value")
                .EnumerateArray()
                .Select(c => new Control
                {
                    Name = c.GetProperty("name").GetString(),
                    Version = new Version(c.GetProperty("version").GetString()),
                    Id = c.GetProperty("customcontrolid").GetGuid(),
                });
        }

        private static async Task<Version> GetPlatformVersionAsync(IPage page)
        {
            return new Version(await page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().getVersion()"));
        }

        private class Control
        {
            public Guid Id { get; set; }

            public Version Version { get; set; }

            public string Name { get; set; }
        }

        private class EnvironmentInfo
        {
            public Version PlatformVersion { get; set; }

            public IDictionary<string, Version> ControlVersions { get; set; }

            public IDictionary<string, Guid> ControlIds { get; set; }
        }
    }
}