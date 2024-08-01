namespace PowerPlaywright.Notifications
{
    using MediatR;
    using PowerPlaywright.Model.Controls;

    /// <summary>
    /// A notification that is published when a control strategy resolver is ready.
    /// </summary>
    public class ControlStrategyResolverReadyNotification : INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlStrategyResolverReadyNotification"/> class.
        /// </summary>
        /// <param name="controlStrategyResolver">The control strategy resolver.</param>
        public ControlStrategyResolverReadyNotification(IControlStrategyResolver controlStrategyResolver)
        {
            this.ControlStrategyResolver = controlStrategyResolver;
        }

        /// <summary>
        /// Gets the control strategy resolver.
        /// </summary>
        public IControlStrategyResolver ControlStrategyResolver { get; }
    }
}
