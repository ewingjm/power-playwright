namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;

    /// <summary>
    /// A strategy resolver for PCF controls.
    /// </summary>
    internal class PcfControlStrategyResolver : AppControlStrategyResolver
    {
        private IDictionary<string, Version> controlVersions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public PcfControlStrategyResolver(ILogger<PcfControlStrategyResolver> logger = null)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        public override bool IsResolvable(Type controlType)
        {
            if (controlType is null)
            {
                throw new ArgumentNullException(nameof(controlType));
            }

            return controlType.GetCustomAttribute<PcfControlAttribute>() != null;
        }

        /// <inheritdoc/>
        public override Type Resolve(Type controlType, IEnumerable<Type> strategyTypes)
        {
            if (controlType is null)
            {
                throw new ArgumentNullException(nameof(controlType));
            }

            if (strategyTypes is null)
            {
                throw new ArgumentNullException(nameof(strategyTypes));
            }

            if (this.controlVersions is null)
            {
                throw new PowerPlaywrightException($"The {nameof(PcfControlStrategyResolver)} must be initialised before it can resolve controls");
            }

            if (controlType.GetCustomAttribute<PcfControlAttribute>() is PcfControlAttribute control)
            {
                return strategyTypes
                    .Where(s => controlType.IsAssignableFrom(s) && !s.IsAbstract && s.IsClass && s.IsVisible)
                    .Select(s =>
                        new
                        {
                            Type = s,
                            s.GetCustomAttribute<PcfControlStrategyAttribute>().Version,
                        })
                    .Where(s => s.Version <= this.controlVersions[control.Name])
                    .OrderByDescending(s => s.Version)
                    .FirstOrDefault()?.Type;
            }

            throw new PowerPlaywrightException($"No supported attributes were found for control type {controlType.Name}. {nameof(PcfControlStrategyResolver)} resolver is unable to resolve the control strategy.");
        }

        /// <inheritdoc/>
        protected override async Task InitialiseResolverAsync(IPage page)
        {
            this.controlVersions = await this.GetControlVersions(page);
        }

        private async Task<IDictionary<string, Version>> GetControlVersions(IPage page)
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
    }
}