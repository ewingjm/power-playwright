﻿namespace PowerPlaywright.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Model.Controls.Pcf;
    using PowerPlaywright.Notifications;

    /// <summary>
    /// A strategy resolver for PCF controls.
    /// </summary>
    internal class PcfControlStrategyResolver : AppControlStrategyResolver
    {
        private IDictionary<string, Version> controlVersions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        public PcfControlStrategyResolver(IMediator mediator, ILogger<PcfControlStrategyResolver> logger = null)
            : base(mediator, logger)
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

            throw new NotSupportedException($"No supported attributes were found for control type {controlType.Name}. {nameof(PcfControlStrategyResolver)} resolver is unable to resolve the control strategy.");
        }

        /// <inheritdoc/>
        protected override async Task Initialise(AppInitializedNotification notification)
        {
            this.controlVersions = await this.GetControlVesions(notification.HomePage.Page);
        }

        private async Task<IDictionary<string, Version>> GetControlVesions(IPage page)
        {
            var customControlsResponse = await page.APIRequest.GetAsync($"https://{new Uri(page.Url).Host}/api/data/v9.2/customcontrols?$select=name,version");
            var customControlsJson = await customControlsResponse.JsonAsync();

            if (!customControlsResponse.Ok)
            {
                throw new PowerPlaywrightException($"Unable to retrieve custom controls from environment: {customControlsJson?.GetProperty("error").GetProperty("message").GetString()}");
            }

            return customControlsJson?.GetProperty("value").EnumerateArray().ToDictionary(
                c => c.GetProperty("name").GetString(),
                c => new Version(c.GetProperty("version").GetString()));
        }
    }
}
