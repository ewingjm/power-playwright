namespace PowerPlaywright.Strategies.Controls.Platform
{
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
    /// An assign dialog.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class AssignDialog : Control, IAssignDialog
    {
        private const string UserOrTeamFieldName = "rdoMe_0";
        private const string AssignToFieldName = "rdoDefaultUser";

        private readonly ILookup userOrTeam;
        private readonly IYesNo assignToChoice;
        private readonly ILocator assignButton;
        private readonly ILocator cancelButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public AssignDialog(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            this.userOrTeam = controlFactory.CreateCachedInstance<ILookup>(appPage, UserOrTeamFieldName, this);
            this.assignToChoice = controlFactory.CreateCachedInstance<IYesNo>(appPage, AssignToFieldName, this);

            this.assignButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Assign" });
            this.cancelButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel" });
        }

        /// <inheritdoc/>
        public async Task<bool> IsVisibleAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.Container.IsVisibleAsync();
        }

        /// <inheritdoc/>
        public async Task CancelAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.cancelButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task AssignToMeAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.assignToChoice.SetValueAsync(false);

            await this.assignButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task AssignToUserOrTeamAsync(string userOrTeam)
        {
            await this.Page.WaitForAppIdleAsync();

            await this.assignToChoice.SetValueAsync(true);
            await this.Page.WaitForAppIdleAsync();
            await this.userOrTeam.SetValueAsync(userOrTeam);

            await this.assignButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("[data-id='Assign'][role='dialog']:not([aria-hidden='true'])");
        }
    }
}
