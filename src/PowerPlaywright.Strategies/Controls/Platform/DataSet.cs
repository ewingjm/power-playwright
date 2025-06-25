namespace PowerPlaywright.Strategies.Controls.Platform
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A data set.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class DataSet : Control, IDataSet
    {
        private readonly IControlFactory controlFactory;
        private readonly string name;
        private readonly IControl parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// <param name="appPage">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="name">The name of the data set.</param>
        /// <param name="parent">The parent control.</param>
        public DataSet(IAppPage appPage, IControlFactory controlFactory, string name, IControl parent = null)
            : base(appPage)
        {
            this.controlFactory = controlFactory;
            this.name = name;
            this.parent = parent;
        }

        /// <inheritdoc/>
        public TPcfControl GetControl<TPcfControl>()
            where TPcfControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<TPcfControl>(this.AppPage, this.name, this);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-id=\"DataSetHostContainer\"][id$=\"{this.name}\"]");
        }
    }
}