namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IDecimalNumber"/> control.
    /// </summary>
    public class DecimalNumberRedirector : ControlRedirector<IDecimalNumber>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalNumberRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public DecimalNumberRedirector(IRedirectionInfoProvider infoProvider, ILogger<DecimalNumberRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            if (!environmentInfo.IsNewLookEnabled)
            {
                return typeof(IDecimalNumberControl);
            }

            return typeof(INumericInput);
        }
    }
}