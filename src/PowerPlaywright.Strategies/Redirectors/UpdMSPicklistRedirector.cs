namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="IUpdMSPicklist"/> control.
    /// </summary>
    public class UpdMSPicklistRedirector : ControlRedirector<IUpdMSPicklist>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdMSPicklistRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public UpdMSPicklistRedirector(IRedirectionInfoProvider infoProvider, ILogger<UpdMSPicklistRedirector> logger)
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