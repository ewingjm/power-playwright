namespace PowerPlaywright.Strategies.Redirectors
{
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Redirects requests for an <see cref="IChoice"/> control.
    /// </summary>
    public class OptionSetRedirector : ControlRedirector<IChoice>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSetRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public OptionSetRedirector(IRedirectionInfoProvider infoProvider, ILogger<OptionSetRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            return typeof(IOptionSetControl);
        }
    }
}