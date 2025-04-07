namespace PowerPlaywright.TestApp.PageObjects
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// A redirector for the custom control class.
    /// </summary>
    public class CustomClassRedirector : ControlRedirector<ICustomControlClass>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomClassRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public CustomClassRedirector(IRedirectionInfoProvider infoProvider, ILogger<CustomClassRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionInfo redirectionInfo)
        {
            return typeof(ICustomControl);
        }
    }
}
