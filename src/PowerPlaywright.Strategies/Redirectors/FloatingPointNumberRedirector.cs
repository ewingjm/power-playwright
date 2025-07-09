namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IFloatingPointNumber"/> control.
    /// </summary>
    public class FloatingPointNumberRedirector : ControlRedirector<IFloatingPointNumber>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingPointNumberRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public FloatingPointNumberRedirector(IRedirectionInfoProvider infoProvider, ILogger<FloatingPointNumberRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            if (!environmentInfo.IsNewLookEnabled)
            {
                return typeof(IFloatingPointNumberControl);
            }

            return typeof(INumericInput);
        }
    }
}