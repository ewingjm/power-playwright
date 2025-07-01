namespace PowerPlaywright.Strategies.Controls.Platform
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using System.Threading.Tasks;

    /// <summary>
    /// A strategy for form fields.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class FormField : Control, IFormField
    {
        private const string FieldRequirementLevelMandatory = "2";

        private readonly string name;

        private readonly ILocator label;
        private readonly ILocator dataSetHostContainer;
        private readonly ILocator dataSetLabel;
        private readonly ILocator fieldSectionItemContainer;
        private readonly ILocator lockedIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormField{TControl}"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The control name.</param>
        /// <param name="parent">The parent control.</param>
        public FormField(IAppPage appPage, string name, IControl parent = null)
            : base(appPage, parent)
        {
            this.name = name;

            this.label = this.Container.Locator("label[id*='field-label']");
            this.dataSetHostContainer = this.Container.Locator("div[data-id='DataSetHostContainer']");
            this.dataSetLabel = this.dataSetHostContainer.Locator("h3");
            this.fieldSectionItemContainer = this.Container.Locator("div[data-id$='-FieldSectionItemContainer']");
            this.lockedIcon = this.Container.Locator("div[data-id$='-locked-icon']");
        }

        /// <inheritdoc/>
        public async Task<string> GetLabelAsync()
        {
            await this.AppPage.Page.WaitForAppIdleAsync();

            if (await this.label.IsVisibleAsync())
            {
                return await this.label.InnerTextAsync();
            }

            if (await this.dataSetLabel.IsVisibleAsync())
            {
                return await this.label.InnerTextAsync();
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public string Name => this.name;

        /// <inheritdoc/>
        public async Task<bool> IsDisabledAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            if (this.Parent is IMainForm form && await form.IsDisabledAsync())
            {
                return true;
            }

            return await this.lockedIcon.IsVisibleAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsMandatoryAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            if (await this.fieldSectionItemContainer.IsVisibleAsync())
            {
                var fieldRequirement = await this.fieldSectionItemContainer.GetAttributeAsync(Attributes.DataFieldRequirement);

                return fieldRequirement == FieldRequirementLevelMandatory;
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task<bool> IsVisibleAsync()
        {
            await this.AppPage.Page.WaitForAppIdleAsync();

            return await this.Container.IsVisibleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-control-name='{this.name}']");
        }
    }
}
