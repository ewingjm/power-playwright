namespace PowerPlaywright.Pages
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A base class for all pages.
    /// </summary>
    internal abstract class AppPage : IAppPage, IAppPageInternal
    {
        private readonly IControlFactory controlFactory;
        private readonly IDictionary<(Type, string), IControl> controlCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public AppPage(IPage page, IControlFactory controlFactory)
        {
            this.controlFactory = controlFactory ?? throw new ArgumentNullException(nameof(controlFactory));
            this.Page = page ?? throw new ArgumentNullException(nameof(page));

            this.controlCache = new Dictionary<(Type, string), IControl>();
        }

        /// <inheritdoc/>
        public event EventHandler OnDestroy;

        /// <summary>
        /// Gets the page.
        /// </summary>
        public IPage Page { get; }

        /// <inheritdoc/>
        public void Destroy()
        {
            // TODO: Call this method on page navigation in PageFactory.
            this.OnDestroy.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets an instance of the control type. If the control is already created, it will be returned from the cache.
        /// </summary>
        /// <typeparam name="TControl">The type of control.</typeparam>
        /// <param name="name">The name of the control (optional).</param>
        /// <returns>The control.</returns>
        protected TControl GetControl<TControl>(string name = null)
            where TControl : IControl
        {
            var cacheKey = (typeof(TControl), name);

            if (this.controlCache.TryGetValue(cacheKey, out var control))
            {
                return (TControl)control;
            }

            control = this.controlFactory.CreateInstance<TControl>(this, name);

            this.controlCache.Add(cacheKey, control);

            return (TControl)control;
        }
    }
}