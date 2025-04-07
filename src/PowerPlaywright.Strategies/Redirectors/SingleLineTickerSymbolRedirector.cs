namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="ISingleLineTickerSymbol"/> control.
    /// </summary>
    public class SingleLineTickerSymbolRedirector : ControlRedirector<ISingleLineTickerSymbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ISingleLineTickerSymbol"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public SingleLineTickerSymbolRedirector(IRedirectionInfoProvider<RedirectionInfo> infoProvider, ILogger<ISingleLineTickerSymbol> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(RedirectionInfo redirectionInfo)
        {
            if (redirectionInfo.ActiveReleaseChannel == ReleaseChannel.SemiAnnualChannel && !redirectionInfo.IsNewLookEnabled)
            {
                return typeof(ITickerSymbolControl);
            }

            return typeof(IActionInput);
        }
    }
}