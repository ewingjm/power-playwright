namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using Bogus;
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
            var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

            await deactivateDialog.CancelAsync();

            Assert.That(await deactivateDialog.IsVisibleAsync(), Is.False);
        }

        ///// <summary>
        ///// Tests that <see cref="ISetStateDialog.DeactivateAsync"/> closes the dialog when it is visible.
        ///// </summary>
        ///// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        //[Test]
        //public async Task DeactivateAsync_DialogIsVisible_Closes()
        //{
        //    var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();

        //    await deactivateDialog.DeactivateAsync();

        //    Assert.That(await deactivateDialog.IsVisibleAsync(), Is.False);
        //}

        /// <summary>
        /// Tests that <see cref="ISetStateDialog.DeactivateAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        //[Test]
        //public async Task SetValueAsync_ContainsValue_ReplacesValue()
        //{
        //    var deactivateDialog = await this.SetupDeactivateDialogScenarioAsync();
        //    var option = await deactivateDialog.GetValueAsync();

        //    var expectedValue = pp_record_statuscode.Inactive.ToDisplayName();

        //    await deactivateDialog.SetValueAsync(expectedValue);

        //    Assert.That(await deactivateDialog.GetValueAsync(), Is.EqualTo(expectedValue));


        //    //var choiceControl = await this.SetupChoiceScenarioAsync();
        //    //var expectedValue = this.faker.PickRandom<pp_record_pp_choice>().ToDisplayName();

        //    //await choiceControl.SetValueAsync(expectedValue);

        //    //Assert.That(choiceControl.GetValueAsync, Is.EqualTo(expectedValue));
        //}

        /// <summary>
        /// Sets up an error dialog scenario.
        /// </summary>
        private async Task<ISetStateDialog> SetupDeactivateDialogScenarioAsync(Faker<pp_Record>? withRecord = null)
        {
            var record = withRecord ?? new Faker<pp_Record>().Generate();

            var recordPage = await this.LoginAndNavigateToRecordAsync(record);

            await recordPage.Form.CommandBar.ClickCommandAsync("Deactivate");

            return recordPage.SetStateDialog;
        }
    }
}