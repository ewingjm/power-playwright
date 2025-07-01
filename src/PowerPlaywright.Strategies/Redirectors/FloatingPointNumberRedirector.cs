namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

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
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            if (!redirectionInfo.IsNewLookEnabled)
            {
                return typeof(IFloatingPointNumberControl);
            }

            return typeof(INumericInput);
        }
    }
}