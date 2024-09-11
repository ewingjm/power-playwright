namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IReadOnlyGrid"/> control.
    /// </summary>
    public class ReadOnlyGridRedirector : ControlRedirector<IReadOnlyGrid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyGridRedirector"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ReadOnlyGridRedirector(ILogger<ReadOnlyGridRedirector> logger = null)
            : base(logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(ControlRedirectionInfo redirectionInfo)
        {
            if (redirectionInfo.AppToggles is null || redirectionInfo.AppToggles.ModernizationOptOut.GetValueOrDefault(true))
            {
                return typeof(IPowerAppsOneGridControl);
            }

            return typeof(IPcfGridControl);
        }
    }
}
