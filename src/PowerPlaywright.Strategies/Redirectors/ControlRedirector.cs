namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// A control redirector.
    /// </summary>
    /// <typeparam name="TSourceControl">The type of control redirected from.</typeparam>
    public abstract class ControlRedirector<TSourceControl> : IControlRedirector<TSourceControl>
        where TSourceControl : IControl
    {
        private readonly IRedirectionInfoProvider<RedirectionInfo> infoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlRedirector{TSourceControl}"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        protected ControlRedirector(IRedirectionInfoProvider<RedirectionInfo> infoProvider, ILogger logger = null)
        {
            this.infoProvider = infoProvider;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <inheritdoc/>
        public Type Redirect()
        {
            this.Logger.LogTrace("Redirecting requested control type: {source}.", typeof(TSourceControl).Name);
            var targetControl = this.GetTargetControlType(this.infoProvider.GetRedirectionInfo());
            this.Logger.LogTrace("Found target control type: {source}.", targetControl.Name);

            return targetControl;
        }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        /// <param name="redirectionInfo">The redirection info.</param>
        /// <returns>The target type.</returns>
        protected abstract Type GetTargetControlType(RedirectionInfo redirectionInfo);
    }
}