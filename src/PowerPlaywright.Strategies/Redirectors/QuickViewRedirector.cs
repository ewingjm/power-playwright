namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="IQuickView"/> control.
    /// </summary>
    public class QuickViewRedirector : ControlRedirector<IQuickView>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuickViewRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public QuickViewRedirector(IRedirectionInfoProvider infoProvider, ILogger<QuickViewRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            return typeof(IQuickForm);
        }
    }
}