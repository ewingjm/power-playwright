namespace PowerPlaywright.Framework.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls;

    /// <summary>
    /// A control redirector.
    /// </summary>
    /// <typeparam name="TSourceControl">The type of control redirected from.</typeparam>
    public abstract class ControlRedirector<TSourceControl> : IControlRedirector<TSourceControl>
        where TSourceControl : IControl
    {
        private readonly IRedirectionInfoProvider infoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlRedirector{TSourceControl}"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        protected ControlRedirector(IRedirectionInfoProvider infoProvider, ILogger logger)
        {
            this.infoProvider = infoProvider;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <inheritdoc/>
        public Type Redirect(RedirectionControlInfo controlInfo)
        {
            this.Logger.LogTrace("Redirecting requested control type: {source}.", typeof(TSourceControl).Name);
            var targetControl = this.GetTargetControlType(this.infoProvider.GetRedirectionInfo(), controlInfo);
            this.Logger.LogTrace("Found target control type: {source}.", targetControl.Name);

            return targetControl;
        }

        /// <summary>
        /// Gets the target type.
        /// </summary>
        /// <param name="environmentInfo">The environment info.</param>
        /// <param name="controlInfo">The control info.</param>
        /// <returns>The target type.</returns>
        protected abstract Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo);
    }
}