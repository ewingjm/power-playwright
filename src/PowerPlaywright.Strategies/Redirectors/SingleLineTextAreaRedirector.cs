﻿namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="ISingleLineTextArea"/> control.
    /// </summary>
    public class SingleLineTextAreaRedirector : ControlRedirector<ISingleLineTextArea>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleLineTextAreaRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public SingleLineTextAreaRedirector(IRedirectionInfoProvider infoProvider, ILogger<SingleLineTextAreaRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            if (!environmentInfo.IsNewLookEnabled)
            {
                return typeof(ITextBoxControl);
            }

            return typeof(ITextInput);
        }
    }
}