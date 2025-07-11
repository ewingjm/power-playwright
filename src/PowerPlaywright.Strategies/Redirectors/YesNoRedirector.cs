﻿namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IYesNo"/> control.
    /// </summary>
    public class YesNoRedirector : ControlRedirector<IYesNo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YesNoRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public YesNoRedirector(IRedirectionInfoProvider infoProvider, ILogger<YesNoRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            if (!environmentInfo.IsNewLookEnabled)
            {
                return typeof(ICheckboxControl);
            }

            return typeof(IOptionSetControl);
        }
    }
}