namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IWholeNumberTimezone"/> control.
    /// </summary>
    public class WholeNumberTimezoneRedirector : ControlRedirector<IWholeNumberTimezone>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WholeNumberTimezoneRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public WholeNumberTimezoneRedirector(IRedirectionInfoProvider infoProvider, ILogger<WholeNumberTimezoneRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            if (!environmentInfo.IsNewLookEnabled)
            {
                return typeof(ITimeZonePickListControl);
            }

            return typeof(IOptionSetControl);
        }
    }
}
