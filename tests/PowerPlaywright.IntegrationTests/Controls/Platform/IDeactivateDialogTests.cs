namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.TestApp.Model;

    /// <summary>
    /// Tests for the <see cref="IDeactivateDialogTests"/> control.
    /// </summary>
    public class IDeactivateDialogTests : IntegrationTests
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
        /// Tests that <see cref="IDeactiveDialog.GetTitleAsync"/> returns the title when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetTitleAsync_DialogIsVisible_ReturnsTitle()
        {
            var expectedTitle = "Confirm Deactivation";
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            Assert.That(await deactivateDialog.GetTitleAsync(), Is.EqualTo(expectedTitle));
        }

        /// <summary>
        /// Tests that <see cref="IDeactiveDialog.GetTextAsync"/> returns the text when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetTextAsync_DialogIsVisible_ReturnsConfirmationText()
        {
            var expectedText = "Do you want to deactivate the selected 1 Record? You can reactivate it later, if you wish.";
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            Assert.That(await deactivateDialog.GetTextAsync(), Is.EqualTo(expectedText));
        }

        /// <summary>
        /// Tests that <see cref="IDeactiveDialog.GetActionTextAsync"/> returns the text when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetActionTextAsync_DialogIsVisible_ReturnsActionText()
        {
            var expectedText = "This action will change the status of the selected Record to Inactive.";
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            Assert.That(await deactivateDialog.GetActionTextAsync(), Is.EqualTo(expectedText));
        }

        /// <summary>
        /// Tests that <see cref="IDeactiveDialog.IsVisibleAsync"/> returns true when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_DialogIsVisible_ReturnsTrue()
        {
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            Assert.That(await deactivateDialog.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IDeactiveDialog.CloseAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CloseAsync_DialogIsVisible_Closes()
        {
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            await deactivateDialog.CloseAsync();

            Assert.That(await deactivateDialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IDeactiveDialog.CancelAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CancelAsync_DialogIsVisible_Closes()
        {
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            await deactivateDialog.CancelAsync();

            Assert.That(await deactivateDialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IDeactiveDialog.DeactivateAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task DeactivateAsync_DialogIsVisible_Closes()
        {
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            await deactivateDialog.DeactivateAsync();

            Assert.That(await deactivateDialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Sets up an error dialog scenario.
        /// </summary>
        private async Task<IDeactiveDialog> SetupDeactivateDialogScenarioAsync(Faker<pp_Record>? withRecord = null)
        {
            var record = withRecord ?? new Faker<pp_Record>().Generate();

            var recordPage = await this.LoginAndNavigateToRecordAsync(record);

            await recordPage.Form.CommandBar.ClickCommandAsync("Deactivate");

            return recordPage.DeactiveDialog;
        }
    }
}