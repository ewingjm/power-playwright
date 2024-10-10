namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// A redirector for the <see cref="ILookup"/> control class.
    /// </summary>
    public class LookupRedirector : ControlRedirector<ILookup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookupRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">the logger.</param>
        public LookupRedirector(IRedirectionInfoProvider<RedirectionInfo> infoProvider, ILogger<LookupRedirector> logger = null)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(RedirectionInfo redirectionInfo)
        {
            return typeof(ISimpleLookupControl);
        }
    }
}