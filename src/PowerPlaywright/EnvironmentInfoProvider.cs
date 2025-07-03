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

            var controls = await GetControlsAsync(page);
            this.ControlIds = controls.ToDictionary(c => c.Name, c => c.Id);
            this.ControlVersions = controls.ToDictionary(c => c.Name, c => c.Version);

            this.PlatformVersion = await GetPlatformVersionAsync(page);

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
    }
}