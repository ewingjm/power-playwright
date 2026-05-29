namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A business process flow.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class BusinessProcessFlow : Control, IBusinessProcessFlow
    {
        private readonly IControlFactory controlFactory;

        private readonly ILocator activeStage;
        private readonly ILocator allStages;
        private readonly ILocator flyout;
        private readonly ILocator nextbutton;
        private readonly ILocator previousButton;
        private readonly ILocator finishButton;
        private readonly ILocator finishedButton;
        private readonly ILocator activeButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessProcessFlow"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public BusinessProcessFlow(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            this.controlFactory = controlFactory;

            this.activeStage = this.Container.Locator("li[data-selected-stage='true']");
            this.allStages = this.Container.Locator("li");
            this.activeButton = this.activeStage.GetByRole(AriaRole.Button);
            this.flyout = appPage.Page.Locator("[data-id^='MscrmControls.Containers.ProcessStageControl-processHeaderStageFlyoutContainer']");
            this.nextbutton = this.flyout.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions() { Name = "Next Stage" });
            this.previousButton = this.flyout.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions() { Name = "Back" });
            this.finishButton = this.flyout.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions() { Name = "Finish" });
            this.finishedButton = this.flyout.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions() { Name = "Finished" });
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetStagesAsync()
        {
            var stageLocator = this.Container.Locator(
                "[data-id^='MscrmControls.Containers.ProcessBreadCrumb-processHeaderStageName_']");

            var stages = await stageLocator.AllTextContentsAsync();

            return stages
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s));
        }

        /// <inheritdoc/>
        public async Task<bool> IsProcessCompleteAsync()
        {
            await this.allStages.Last.ClickAndWaitForAppIdleAsync();
            return await this.finishedButton.IsVisibleAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> NextAsync()
        {
            return await this.MoveStage(this.nextbutton);
        }

        /// <inheritdoc/>
        public async Task<bool> PreviousAsync()
        {
            return await this.MoveStage(this.previousButton);
        }

        /// <inheritdoc/>
        public async Task<string> GetCurrentStageAsync()
        {
            if (await this.activeStage.CountAsync() == 0)
            {
                throw new PowerPlaywrightException(
                    "Could not find the current stage. Ensure BPF is enabled.");
            }

            var nameLocator = this.activeStage.Locator(
                "div[data-id^='MscrmControls.Containers.ProcessBreadCrumb-processHeaderStageName_']");

            return (await nameLocator.InnerTextAsync()).Trim();
        }

        /// <inheritdoc/>
        public async Task<bool> CompleteAsync()
        {
            if (!await this.finishButton.IsVisibleAsync())
            {
                throw new PowerPlaywrightException("Finish button is not visible. Ensure you are on the last stage of the business process flow.");
            }

            await this.finishButton.ClickAndWaitForAppIdleAsync();
            return await this.IsProcessCompleteAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsFinalStageAsync()
        {
            var stages = await this.GetStagesAsync();
            var currentStage = await this.GetCurrentStageAsync();

            return currentStage == stages.Last();
        }

        /// <inheritdoc/>
        public async Task ExecuteStageActionAsync(Func<IEnumerable<IField>, Task> action)
        {
            var stage = this.GetStage(await this.GetCurrentStageAsync());

            if (!await this.flyout.IsVisibleAsync())
            {
                await stage.ClickAndWaitForAppIdleAsync();
            }

            var fieldLocators = await this.flyout.Locator("div[data-id*='MscrmControls.Containers.ProcessStageControl-fieldSectionItemContainer'] [data-lp-id*='MscrmControls.Containers.FieldSectionItem']").AllAsync();

            var lpIdTasks = fieldLocators
                .Select(field => field.GetAttributeAsync(Attributes.DataLpId))
                .ToArray();

            var lpIds = await Task.WhenAll(lpIdTasks);

            var fields = lpIds
                .Select(lpId =>
                {
                    var fieldName = lpId.Split('|')[1];
                    return this.controlFactory.CreateCachedInstance<IField>(this.AppPage, fieldName);
                })
                .ToList();

            await action(fields);

            await this.CollapseFlyoutAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("#bpfContainer");
        }

        private async Task<bool> MoveStage(ILocator button)
        {
            var currentStage = await this.GetCurrentStageAsync();
            await this.activeButton.ClickAsync();

            if (!await button.IsVisibleAsync())
            {
                return false;
            }

            await this.flyout.WaitForAsync();
            await button.ClickAsync();
            await this.Page.WaitForAppIdleAsync();
            var newStage = await this.GetCurrentStageAsync();

            return currentStage != newStage;
        }
 
        private async Task CollapseFlyoutAsync()
        {
            if (await this.flyout.IsVisibleAsync())
            {
                var closeButton = this.flyout.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions() { Name = "Close", Exact = true });
                await closeButton.ClickAndWaitForAppIdleAsync();
            }
        }

        private ILocator GetStage(string stageName)
        {
            var stageLocator = this.Container.Locator(
                "li[id^='MscrmControls.Containers.ProcessBreadCrumb-processHeaderStage']",
                new LocatorLocatorOptions { HasTextString = stageName });

            return stageLocator;
        }
    }
}