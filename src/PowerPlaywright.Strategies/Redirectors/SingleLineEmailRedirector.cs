namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="ISingleLineEmail"/> control.
    /// </summary>
    public class SingleLineEmailRedirector : ControlRedirector<ISingleLineEmail>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ISingleLineEmail"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public SingleLineEmailRedirector(IRedirectionInfoProvider infoProvider, ILogger<ISingleLineEmail> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            if (redirectionInfo.ActiveReleaseChannel == (int)ReleaseChannel.SemiAnnualChannel && !redirectionInfo.IsNewLookEnabled)
            {
                return typeof(IEmailAddressControl);
            }

            return typeof(IActionInput);
        }
    }
}