namespace PowerPlaywright.Model.Controls
{
    using Microsoft.Playwright;

    /// <summary>
    /// A base class that all controls must inherit from.
    /// </summary>
    public abstract class Control : IControl
    {
        private ILocator container;

        /// <summary>
        /// Initializes a new instance of the <see cref="Control"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="parent">The parent conrol.</param>
        protected Control(IPage page, IControl parent = null)
        {
            this.Page = page;
            this.Parent = parent;
        }

        /// <inheritdoc/>
        public IControl Parent { get; }

        /// <inheritdoc/>
        public ILocator Container
        {
            get
            {
                if (this.container is null)
                {
                    this.container = this.Parent is null ? this.GetContainer() : this.Parent.Container.Locator(this.GetContainer());
                }

                return this.container;
            }
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>The page.</value>
        protected IPage Page { get; }

        /// <summary>
        /// Get the control container.
        /// </summary>
        /// <returns>The container.</returns>
        protected abstract ILocator GetContainer();
    }
}