namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IChoices"/> control.
    /// </summary>
    public class ChoicesRedirector : ControlRedirector<IChoices>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChoicesRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public ChoicesRedirector(IRedirectionInfoProvider infoProvider, ILogger<ChoicesRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            return typeof(IUpdMSPicklistControl);
        }
    }
}