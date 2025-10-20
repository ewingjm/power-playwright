namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IAssignDialog"/> control.
    /// </summary>
    public class IAssignDialogTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the test dependencies.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }

        /// <summary>
        /// Tests that <see cref="IAssignDialog.IsVisibleAsync"/> returns true when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_AssignDialogIsVisible_ReturnsTrue()
        {
            var (dialog, _) = await this.SetupAssignDialogScenarioAsync();

            Assert.That(await dialog.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IAssignDialog.CancelAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CancelAsync_AssignDialogIsVisible_Closes()
        {
            var (dialog, _) = await this.SetupAssignDialogScenarioAsync();

            await dialog.CancelAsync();

            Assert.That(await dialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IAssignDialog.AssignToMeAsync"/> assigns the record to the current user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task AssignToMeAsync_AssignToMe_AssignsToCurrentUser()
        {
            var (dialog, recordPage) = await this.SetupAssignDialogScenarioAsync();
            var currentUserName = await recordPage.Page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().userSettings.userName");

            await dialog.AssignToMeAsync();

            var ownerIdLookup = recordPage.Form.GetField<ILookup>(nameof(pp_Record.OwnerId).ToLower()).Control;

            Assert.That(ownerIdLookup.GetValueAsync, Is.EqualTo(currentUserName));
        }

        /// <summary>
        /// Tests that <see cref="IAssignDialog.AssignToUserOrTeamAsync"/> assigns the record to a specified user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task AssignToUserOrTeamAsync_AssignToUser_AssignsToSpecifiedUser()
        {
            var (dialog, recordPage) = await this.SetupAssignDialogScenarioAsync();
            var targetUser = await recordPage.Page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().userSettings.userName");

            await dialog.AssignToUserOrTeamAsync(targetUser);

            var ownerIdLookup = recordPage.Form.GetField<ILookup>(nameof(pp_Record.OwnerId).ToLower()).Control;

            Assert.That(ownerIdLookup.GetValueAsync, Is.EqualTo(targetUser));
        }

        private async Task<(IAssignDialog Dialog, IEntityRecordPage RecordPage)> SetupAssignDialogScenarioAsync()
        {
            var record = new RecordFaker().Generate();

            var recordPage = await this.LoginAndNavigateToRecordAsync(record);

            var dialog = await recordPage.Form.CommandBar.ClickCommandWithDialogAsync<IAssignDialog>("Assign");

            return (dialog, recordPage);
        }
    }
}
