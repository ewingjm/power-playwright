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
    public class BusinessProcessFlow : Control, IBusinessProcessFlow
    {
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
        /// <param name="parent">The parent control.</param>
        public BusinessProcessFlow(IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
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

            return currentStage != stages.Last();
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
            await this.flyout.WaitForAsync();
            await button.ClickAsync();
            await this.Page.WaitForAppIdleAsync();
            var newStage = await this.GetCurrentStageAsync();

            return currentStage != newStage;
        }
    }
}