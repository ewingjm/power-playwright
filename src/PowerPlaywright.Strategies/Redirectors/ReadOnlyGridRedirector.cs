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
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public ReadOnlyGridRedirector(IRedirectionInfoProvider<RedirectionInfo> infoProvider, ILogger<ReadOnlyGridRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(RedirectionInfo redirectionInfo)
        {
            if (redirectionInfo.ActiveReleaseChannel == ReleaseChannel.SemiAnnualChannel && !redirectionInfo.IsNewLookEnabled)
            {
                return typeof(IPcfGridControl);
            }

            return typeof(IPowerAppsOneGridControl);
        }
    }
}