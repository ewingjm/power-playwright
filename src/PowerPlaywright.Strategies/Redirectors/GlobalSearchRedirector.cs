namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="IGlobalSearch"/> control.
    /// </summary>
    public class GlobalSearchRedirector : ControlRedirector<IGlobalSearch>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YesNoRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public GlobalSearchRedirector(IRedirectionInfoProvider infoProvider, ILogger<GlobalSearchRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            if (!redirectionInfo.IsRelevanceSearchEnabled)
            {
                throw new PowerPlaywrightException("Relevance Search is not enabled for this environment");
            }

            return typeof(IGlobalSearch);
        }
    }
}