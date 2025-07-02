namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IFieldSectionItem"/> interface.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class FieldSectionItem : PcfControlInternal, IFieldSectionItem
    {
        private readonly ILocator label;
        private readonly ILocator lockedIcon;
        private readonly ILocator fieldSectionItemContainer;
        private readonly IControlFactory controlFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldSectionItem"/> class.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control</param>
        public FieldSectionItem(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControlFactory controlFactory, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.label = this.Container.Locator("label[id*='field-label']");
            this.lockedIcon = this.Container.Locator("div[data-id$='-locked-icon']");
            this.fieldSectionItemContainer = this.Container.Locator("div[data-id$='-FieldSectionItemContainer']");
            this.controlFactory = controlFactory;
        }

        /// <inheritdoc/>
        public TPcfControl GetControl<TPcfControl>()
            where TPcfControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<TPcfControl>(this.AppPage, this.Name, this);
        }

        /// <inheritdoc/>
        public async Task<string> GetLabelAsync()
        {
            await this.AppPage.Page.WaitForAppIdleAsync();

            if (!await this.label.IsVisibleAsync())
            {
                return string.Empty;
            }

            return await this.label.InnerTextAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsDisabledAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.lockedIcon.IsVisibleAsync() || this.Parent is IMainForm form && await form.IsDisabledAsync();
        }

        /// <inheritdoc/>
        public async Task<FieldRequirementLevel> GetRequirementLevelAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            if (!await this.fieldSectionItemContainer.IsVisibleAsync())
            {
                return FieldRequirementLevel.None;
            }

            var fieldRequirement = await this.fieldSectionItemContainer.GetAttributeAsync(Attributes.DataFieldRequirement);

            switch (int.Parse(fieldRequirement))
            {
                case 0:
                    return FieldRequirementLevel.None;
                case 1:
                case 2:
                    return FieldRequirementLevel.Required;
                case 3:
                    return FieldRequirementLevel.Recommended;
                default:
                    throw new NotImplementedException($"Unrecognised field requirement level: {fieldRequirement}");
            }
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
            return context.Locator($"//div[((starts-with(@data-lp-id, '{this.PcfControlAttribute.Name}|') or starts-with(@data-lp-id, '{this.GetControlId()}|')) and contains(@data-lp-id, '|{this.Name}|'))]");
        }
    }
}
