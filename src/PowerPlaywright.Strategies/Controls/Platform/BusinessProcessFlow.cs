namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A business process flow.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class BusinessProcessFlow : Control, IBusinessProcess
    {
        private readonly IPageFactory pageFactory;
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
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public BusinessProcessFlow(IAppPage appPage, IPageFactory pageFactory, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            this.pageFactory = pageFactory;
            this.controlFactory = controlFactory;
            this.activeStage = this.Container.Locator("li[data-selected-stage='true']");
            this.allStages = this.Container.Locator("li");
            this.activeButton = this.activeStage.Locator("button");
            this.flyout = appPage.Page.Locator("[data-id^='MscrmControls.Containers.ProcessStageControl-processHeaderStageFlyoutContainer']");
            this.nextbutton = this.flyout.GetByLabel("Next Stage");
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
        public async Task<string> MoveToNextStageAsync()
        {
            return await this.MoveStage(this.nextbutton);
        }

        /// <inheritdoc/>
        public async Task<string> MoveToPreviousStageAsync()
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
        public async Task<bool> HasAnotherStageAsync()
        {
            var stages = await this.GetStagesAsync();
            var currentStage = await this.GetCurrentStageAsync();

            return currentStage != stages.Last();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("#bpfContainer");
        }

        private async Task<string> MoveStage(ILocator button)
        {
            await this.activeButton.ClickAsync();
            await this.flyout.WaitForAsync();
            await button.ClickAsync();
            await this.Page.WaitForAppIdleAsync();
            return await this.GetCurrentStage();
        }

        private async Task<string> GetCurrentStage()
        {
            var currentStage = this.Container.Locator(
               "li[data-selected-stage='true'] " +
               "div[data-id^='MscrmControls.Containers.ProcessBreadCrumb-processHeaderStageName_']");

            if (await currentStage.CountAsync() == 0)
            {
                throw new PowerPlaywrightException("Could not find the current stage. Please ensure you have a BPF enabled on the entity.");
            }

            return (await currentStage.InnerTextAsync()).Trim();
        }
    }
}