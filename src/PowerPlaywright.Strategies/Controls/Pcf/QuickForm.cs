namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A control strategy for the <see cref="QuickForm"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class QuickForm : PcfControlInternal, IQuickForm
    {
        private readonly IControlFactory controlFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickForm"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="infoProvider"> The info provider.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="controlFactory">The control factory.</param>
        public QuickForm(IAppPage appPage, string name, IEnvironmentInfoProvider infoProvider, IControl parent, IControlFactory controlFactory)
            : base(name, appPage, infoProvider, parent)
        {
            this.controlFactory = controlFactory;
        }

        /// <inheritdoc/>
        public IField GetField(string name)
        {
            return this.controlFactory.CreateInstance<IField>(this.AppPage, $"{this.Name}.{name}", this);
        }

        /// <inheritdoc/>
        public IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<IField<TControl>>(this.AppPage, $"{this.Name}.{name}", this);
        }

        /// <inheritdoc/>
        public Task<bool> IsVisibleAsync()
        {
            return this.Container.IsVisibleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"//div[((starts-with(@data-lp-id, '{this.PcfControlAttribute.Name}|') or starts-with(@data-lp-id, '{this.GetControlId()}|')) and contains(@data-lp-id, '|{this.Name}|'))]");
        }
    }
}