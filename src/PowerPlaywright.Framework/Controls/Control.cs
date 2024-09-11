namespace PowerPlaywright.Framework.Controls
{
    using Microsoft.Playwright;

    /// <summary>
    /// A base class that all controls must inherit from.
    /// </summary>
    public abstract class Control : IControl
    {
        private readonly IControl parent;
        private ILocator container;

        /// <summary>
        /// Initializes a new instance of the <see cref="Control"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="parent">The parent conrol.</param>
        protected Control(IPage page, IControl parent = null)
        {
            this.Page = page;
            this.parent = parent;
        }

        /// <summary>
        /// Gets the parent control.
        /// </summary>
        IControl IControl.Parent => this.parent;

        /// <summary>
        /// Gets the container.
        /// </summary>
        ILocator IControl.Container
        {
            get
            {
                if (this.Root is null)
                {
                    return this.parent?.Container ?? this.Page.Locator("html");
                }

                if (this.container is null)
                {
                    this.container = this.Root;
                }

                return this.container;
            }
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        protected ILocator Container => ((IControl)this).Container;

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>The page.</value>
        protected IPage Page { get; }

        /// <summary>
        /// Gets the control root.
        /// </summary>
        /// <returns>The root.</returns>
        protected abstract ILocator Root { get; }
    }
}