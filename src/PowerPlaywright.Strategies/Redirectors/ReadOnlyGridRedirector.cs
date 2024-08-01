namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Model.Controls.Pcf;
    using PowerPlaywright.Model.Controls.Pcf.Classes;
    using PowerPlaywright.Model.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IReadOnlyGrid"/> control.
    /// </summary>
    public class ReadOnlyGridRedirector : IControlRedirector<ControlRedirectionInfo, IReadOnlyGrid>
    {
        private readonly ILogger<ReadOnlyGridRedirector> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyGridRedirector"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ReadOnlyGridRedirector(ILogger<ReadOnlyGridRedirector> logger = null)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Type Redirect(ControlRedirectionInfo redirectionInfo, IEnumerable<Type> controlTypes)
        {
            this.logger.LogTrace("Getting redirected control for control class {class}.", nameof(IReadOnlyGrid));

            if (redirectionInfo.AppToggles.ModernizationOptOut)
            {
                return typeof(IPowerAppsOneGridControl);
            }

            return typeof(IPcfGridControl);
        }
    }
}
