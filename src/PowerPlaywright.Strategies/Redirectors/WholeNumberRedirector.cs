namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IWholeNumber"/> control.
    /// </summary>
    public class WholeNumberRedirector : ControlRedirector<IWholeNumber>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WholeNumberRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public WholeNumberRedirector(IRedirectionInfoProvider infoProvider, ILogger<WholeNumberRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            if (!redirectionInfo.IsNewLookEnabled)
            {
                return typeof(IWholeNumberControl);
            }

            return typeof(INumericInput);
        }
    }
}