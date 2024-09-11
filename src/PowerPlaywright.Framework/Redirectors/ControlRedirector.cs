namespace PowerPlaywright.Framework.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls;

    /// <summary>
    /// A control redirector.
    /// </summary>
    /// <typeparam name="TSourceControl">The type of control redirected from.</typeparam>
    public abstract class ControlRedirector<TSourceControl> : IControlRedirector<ControlRedirectionInfo, TSourceControl>
        where TSourceControl : IControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlRedirector{TSourceControl}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected ControlRedirector(ILogger logger = null)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <inheritdoc/>
        public Type Redirect(ControlRedirectionInfo redirectionInfo)
        {
            this.Logger?.LogTrace("Redirecting requested control type: {source}.", typeof(TSourceControl).Name);
            var targetControl = this.GetTargetControlType(redirectionInfo);
            this.Logger?.LogTrace("Found target control type: {source}.", targetControl.Name);

            return targetControl;
        }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        /// <param name="redirectionInfo">The redirection info.</param>
        /// <returns>The target type.</returns>
        protected abstract Type GetTargetControlType(ControlRedirectionInfo redirectionInfo);
    }
}
