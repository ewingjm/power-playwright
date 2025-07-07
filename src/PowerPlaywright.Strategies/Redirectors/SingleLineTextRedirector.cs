namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="ISingleLineText"/> control.
    /// </summary>
    public class SingleLineTextRedirector : ControlRedirector<ISingleLineText>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleLineTextRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public SingleLineTextRedirector(IRedirectionInfoProvider infoProvider, ILogger<SingleLineTextRedirector> logger)
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