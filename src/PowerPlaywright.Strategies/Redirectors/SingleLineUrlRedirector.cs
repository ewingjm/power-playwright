namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="ISingleLineUrl"/> control.
    /// </summary>
    public class SingleLineUrlRedirector : ControlRedirector<ISingleLineUrl>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleLineUrlRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public SingleLineUrlRedirector(IRedirectionInfoProvider<RedirectionInfo> infoProvider, ILogger<SingleLineUrlRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(RedirectionInfo redirectionInfo)
        {
            if (redirectionInfo.ActiveReleaseChannel == ReleaseChannel.SemiAnnualChannel && !redirectionInfo.IsNewLookEnabled)
            {
                return typeof(IClassicUrlControl);
            }

            return typeof(IUrlControl);
        }
    }
}