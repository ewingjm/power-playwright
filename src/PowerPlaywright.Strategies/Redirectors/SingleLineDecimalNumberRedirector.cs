namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="IDecimal"/> control.
    /// </summary>
    public class SingleLineDecimalNumberRedirector : ControlRedirector<IDecimal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IDecimal"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public SingleLineDecimalNumberRedirector(IRedirectionInfoProvider infoProvider, ILogger<SingleLineDecimalNumberRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            if (redirectionInfo.ActiveReleaseChannel == (int)ReleaseChannel.SemiAnnualChannel && !redirectionInfo.IsNewLookEnabled)
            {
                return typeof(IDecimalNumberControl);
            }

            return typeof(INumericInput);
        }
    }
}