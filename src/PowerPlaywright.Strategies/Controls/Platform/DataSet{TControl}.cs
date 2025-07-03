namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Threading.Tasks;
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
    /// <typeparam name="TControl">The control type.</typeparam>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class DataSet<TControl> : Control, IDataSet<TControl>
        where TControl : IPcfControl
    {
        private readonly IControlFactory controlFactory;
        private readonly string name;
        private readonly IControl parent;

        private readonly IDataSet dataSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet{TControl}"/> class.
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

            this.dataSet = this.controlFactory.CreateInstance<IDataSet>(this.AppPage, this.name, this.parent);
        }

        /// <inheritdoc/>
        public TControl Control => this.controlFactory.CreateInstance<TControl>(this.AppPage, this.name, this);

        /// <inheritdoc/>
        public TPcfControl GetControl<TPcfControl>()
            where TPcfControl : IPcfControl
        {
            return this.dataSet.GetControl<TPcfControl>();
        }

        /// <inheritdoc/>
        public Task<bool> IsVisibleAsync()
        {
            return this.dataSet.IsVisibleAsync();
        }

        /// <inheritdoc/>
        public Task SwitchViewAsync(string viewName)
        {
            return this.dataSet.SwitchViewAsync(viewName);
        }

        /// <inheritdoc/>
        public Task<string> GetActiveViewAsync()
        {
            return this.dataSet.GetActiveViewAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-id=\"DataSetHostContainer\"]{(!string.IsNullOrEmpty(this.name) ? $"[id$=\"{this.name}\"]" : string.Empty)}");
        }
    }
}