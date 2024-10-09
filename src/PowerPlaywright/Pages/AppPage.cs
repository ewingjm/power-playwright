namespace PowerPlaywright.Pages
{
    using System;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A base class for all pages.
    /// </summary>
    internal abstract class AppPage : IAppPage, IAppPageInternal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public AppPage(IPage page, IControlFactory controlFactory)
        {
            this.Page = page ?? throw new ArgumentNullException(nameof(page));
            this.ControlFactory = controlFactory ?? throw new ArgumentNullException(nameof(controlFactory));
        }

        /// <inheritdoc/>
        public event EventHandler OnDestroy;

        /// <summary>
        /// Gets the page.
        /// </summary>
        public IPage Page { get; }

        /// <summary>
        /// Gets the control factory.
        /// </summary>
        protected IControlFactory ControlFactory { get; }

        /// <inheritdoc/>
        public void Destroy()
        {
            // TODO: Call this method on page navigation in PageFactory.
            this.OnDestroy.Invoke(this, EventArgs.Empty);
        }
    }
}