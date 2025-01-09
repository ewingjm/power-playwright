namespace PowerPlaywright.Strategies.Controls
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A control.
    /// </summary>
    public abstract class Control : IControl
    {
        private readonly IControl parent;
        private ILocator container;

        /// <summary>
        /// Initializes a new instance of the <see cref="Control"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent conrol.</param>
        protected Control(IAppPage appPage, IControl parent = null)
        {
            this.AppPage = appPage;
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
                if (this.container is null)
                {
                    this.container = this.GetRoot(this.parent?.Container ?? this.Page.Locator("html"));
                }

                return this.container;
            }
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        protected ILocator Container => ((IControl)this).Container;

        /// <summary>
        /// Gets the app page.
        /// </summary>
        /// <value>The app page.</value>
        protected IAppPage AppPage { get; }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>The page.</value>
        protected IPage Page => this.AppPage.Page;

        /// <summary>
        /// Gets the control root locator.
        /// </summary>
        /// <param name="context">The locator context.</param>
        /// <returns>The control root locator.</returns>
        protected abstract ILocator GetRoot(ILocator context);
    }
}