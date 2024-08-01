namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Model;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Model.Controls.Pcf;
    using PowerPlaywright.Model.Controls.Platform;
    using PowerPlaywright.Model.Controls.Platform.Attributes;

    /// <summary>
    /// A main form.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class MainFormControl : Control, IMainFormControl
    {
        private readonly IControlFactory controlFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFormControl"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public MainFormControl(IPage page, IControlFactory controlFactory)
            : base(page, null)
        {
            this.controlFactory = controlFactory ?? throw new System.ArgumentNullException(nameof(controlFactory));
        }

        /// <inheritdoc/>
        public TControl GetControl<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<TControl>(this.Page, name, this);
        }

        /// <inheritdoc/>
        public Task OpenTabAsync(string name)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        protected override ILocator GetContainer()
        {
            return this.Page.Locator("div[data-id='editFormRoot'");
        }
    }
}
