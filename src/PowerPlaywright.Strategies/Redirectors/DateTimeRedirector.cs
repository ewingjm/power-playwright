namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IDateTime"/> control.
    /// </summary>
    public class DateTimeRedirector : ControlRedirector<IDateTime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public DateTimeRedirector(IRedirectionInfoProvider infoProvider, ILogger<DateTimeRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            return typeof(IDateTimeControl);
        }
    }
}