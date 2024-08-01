namespace PowerPlaywright.Notifications
{
    using PowerPlaywright.Model.Controls;

    /// <summary>
    /// A notification that is published when a control strategy resolver is ready.
    /// </summary>
    internal class ResolverReadyEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolverReadyEvent"/> class.
        /// </summary>
        /// <param name="resolver">The control strategy resolver.</param>
        public ResolverReadyEvent(IControlStrategyResolver resolver)
        {
            this.Resolver = resolver;
        }

        /// <summary>
        /// Gets the control strategy resolver.
        /// </summary>
        public IControlStrategyResolver Resolver { get; }
    }
}
