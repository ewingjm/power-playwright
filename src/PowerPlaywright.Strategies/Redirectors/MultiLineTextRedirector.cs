namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IMultiLineText"/> control.
    /// </summary>
    public class MultiLineTextRedirector : ControlRedirector<IMultiLineText>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLineTextRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public MultiLineTextRedirector(IRedirectionInfoProvider infoProvider, ILogger<MultiLineTextRedirector> logger)
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