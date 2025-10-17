namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.IntegrationTests.Extensions;
    using PowerPlaywright.TestApp.Model;

    /// <summary>
    /// Tests for the <see cref="ISetStateDialogTests"/> control.
    /// </summary>
    public class ISetStateDialogTests : IntegrationTests
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
        /// Tests that <see cref="ISetStateDialog.IsVisibleAsync"/> returns true when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_DialogIsVisible_ReturnsTrue()
        {
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            Assert.That(await deactivateDialog.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that <see cref="ISetStateDialogTests.CloseAsync"/> closes the dialog when it is visible.
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
        /// Tests that <see cref="ISetStateDialog.CancelAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CancelAsync_DialogIsVisible_Closes()
        {
            var dialog = await this.SetupDeactivateDialogScenarioAsync();

            await dialog.CancelAsync();

            Assert.That(await dialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that <see cref="ISetStateDialog.DeactivateAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task DeactivateAsync_DialogIsVisible_Closes()
        {
            var dialog = await this.SetupDeactivateDialogScenarioAsync();

            await dialog.DeactivateAsync();

            Assert.That(await dialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that <see cref="ISetStateDialog.ActivateAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ActivateAsync_DialogIsVisible_Closes()
        {
            var dialog = await this.SetupActivateDialogScenarioAsync();

            await dialog.ActivateAsync();

            Assert.That(await dialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that <see cref="ISetStateDialog.SetValueAsync"/> returns an exception if the record is configured with a single status reason for the given state.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_SingleStatusReason_ThrowsException()
        {
            var dialog = await this.SetupActivateDialogScenarioAsync();

            Assert.ThrowsAsync<PowerPlaywrightException>(
                () => dialog.SetValueAsync(pp_record_statecode.Active.ToDisplayName()));
        }

        /// <summary>
        /// Tests that <see cref="ISetStateDialog.DeactivateAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var dialog = await this.SetupDeactivateDialogScenarioAsync();

            var expectedValue = pp_record_statuscode.Cancelled.ToDisplayName();

            await dialog.SetValueAsync(expectedValue);

            Assert.That(await dialog.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a deactivate dialog scenario.
        /// </summary>
        private async Task<ISetStateDialog> SetupDeactivateDialogScenarioAsync(Faker<pp_Record>? withRecord = null)
        {
            var record = withRecord ?? new Faker<pp_Record>();

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            await recordPage.Form.CommandBar.ClickCommandAsync("Deactivate");

            return recordPage.SetStateDialog;
        }

        /// <summary>
        /// Sets up an activate dialog scenario.
        /// </summary>
        private async Task<ISetStateDialog> SetupActivateDialogScenarioAsync(Faker<pp_Record>? withRecord = null)
        {
            var record = withRecord ?? new Faker<pp_Record>()
                .RuleFor(x => x.pp_singlelineoftexttext, f => string.Join(" ", f.Lorem.Random.Words(5)))
                .RuleFor(x => x.statecode, pp_record_statecode.Inactive)
                .RuleFor(x => x.statuscode, pp_record_statuscode.Cancelled);

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            await recordPage.Form.CommandBar.ClickCommandAsync("Activate");

            return recordPage.SetStateDialog;
        }
    }
}