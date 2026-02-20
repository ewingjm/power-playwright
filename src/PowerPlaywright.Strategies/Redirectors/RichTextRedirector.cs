namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IRichText"/> control.
    /// </summary>
    public class RichTextRedirector : ControlRedirector<IRichText>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public RichTextRedirector(IRedirectionInfoProvider infoProvider, ILogger<RichTextRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            return typeof(IRichTextControl);
        }
    }
}