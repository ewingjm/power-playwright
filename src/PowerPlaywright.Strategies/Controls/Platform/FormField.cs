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
        private readonly string name;
        private readonly string overrideName;

        private readonly ILocator label;
        private readonly ILocator dataSetLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormField{TControl}"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The control name.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="overrideName">The control name locator override</param>
        public FormField(IAppPage appPage, string name, IControl parent = null, string overrideName = null)
            : base(appPage, parent)
        {
            this.name = name;
            this.overrideName = overrideName;

            this.label = this.Container.Locator("label[id*='field-label']");
            var dataSetHostContainer = this.Container.Locator("div[data-id='DataSetHostContainer']");
            this.dataSetLabel = dataSetHostContainer.Locator("h3");
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
        public async Task<bool> IsVisibleAsync()
        {
            await this.AppPage.Page.WaitForAppIdleAsync();

            return await this.Container.IsVisibleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            if (this.overrideName != null)
            {
                return context.Locator($"div[data-control-name='{this.overrideName}']");
            }
            return context.Locator($"div[data-control-name='{this.name}']");
        }
    }
}