namespace PowerPlaywright.Strategies.Controls.Platform
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;
    using System.Threading.Tasks;

    /// <summary>
    /// A strategy for form fields.
    /// </summary>
    /// <typeparam name="TPcfControl">The PCF control type.</typeparam>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class FormField<TPcfControl> : Control, IFormField<TPcfControl>
        where TPcfControl : class, IPcfControl
    {
        private readonly IControlFactory controlFactory;
        private readonly IFormField formField;

        private readonly TPcfControl control;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormField{TControl}"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="name">The control name.</param>
        /// <param name="overrideName">Used when the data-control-id is not matching the field name on rare occassions.</param>
        /// <param name="parent">The parent control.</param>
        public FormField(IControlFactory controlFactory, IAppPage appPage, string name, IControl parent = null, string overrideName = null)
            : base(appPage, parent)
        {
            this.controlFactory = controlFactory;
            this.formField = this.controlFactory.CreateInstance<IFormField>(this.AppPage, name, parent, overrideName);
            this.control = this.controlFactory.CreateInstance<TPcfControl>(this.AppPage, name, this, overrideName);
        }

        /// <inheritdoc/>
        public TPcfControl Control => this.control;

        /// <inheritdoc/>
        public Task<string> GetLabelAsync()
        {
            return this.formField.GetLabelAsync();
        }

        /// <inheritdoc/>
        public string Name => this.formField.Name;

        /// <inheritdoc/>
        public Task<bool> IsVisibleAsync()
        {
            return this.formField.IsVisibleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return this.formField.Container;
        }
    }
}