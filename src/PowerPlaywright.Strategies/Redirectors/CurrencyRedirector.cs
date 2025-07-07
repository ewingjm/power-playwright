namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="ICurrency"/> control.
    /// </summary>
    public class CurrencyRedirector : ControlRedirector<ICurrency>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public CurrencyRedirector(IRedirectionInfoProvider infoProvider, ILogger<CurrencyRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            if (!environmentInfo.IsNewLookEnabled)
            {
                return typeof(ICurrencyControl);
            }

            return typeof(INumericInput);
        }
    }
}