namespace PowerPlaywright.Resolvers
{
    using System;

    /// <summary>
    /// A notification that is published when a control strategy resolver is ready.
    /// </summary>
    internal class ResolverReadyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolverReadyEventArgs"/> class.
        /// </summary>
        /// <param name="resolver">The control strategy resolver.</param>
        public ResolverReadyEventArgs(IControlStrategyResolver resolver)
        {
            this.Resolver = resolver;
        }

        /// <summary>
        /// Gets the control strategy resolver.
        /// </summary>
        public IControlStrategyResolver Resolver { get; }
    }
}
