namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="ISingleLineDecimal"/> control.
    /// </summary>
    public class SingleLineDecimalNumberRedirector : ControlRedirector<ISingleLineDecimal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ISingleLineDecimal"/> class.
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