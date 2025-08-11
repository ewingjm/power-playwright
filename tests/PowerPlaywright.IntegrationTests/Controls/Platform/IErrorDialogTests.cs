namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IErrorDialog"/> control.
    /// </summary>
    public class IErrorDialogTests : IntegrationTests
    {
        /// <summary>
        /// Tests that <see cref="IErrorDialog.GetTextAsync"/> returns the text when the error dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetTextAsync_DialogIsVisible_ReturnsText()
        {
            var errorDialog = await this.SetupErrorDialogScenarioAsync();

            Assert.That(await errorDialog.GetTextAsync(), Is.EqualTo("This is the error text"));
        }

        /// <summary>
        /// Tests that <see cref="IErrorDialog.CancelAsync"/> closes the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CloseAsync_DialogIsVisible_Closes()
        {
            var errorDialog = await this.SetupErrorDialogScenarioAsync();

            await errorDialog.CancelAsync();

            Assert.That(await errorDialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IErrorDialog.IsVisibleAsync"/> returns true when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_DialogIsVisible_ReturnsTrue()
        {
            var errorDialog = await this.SetupErrorDialogScenarioAsync();

            Assert.That(await errorDialog.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Sets up an error dialog scenario.
        /// </summary>
        private async Task<IErrorDialog> SetupErrorDialogScenarioAsync()
        {
            var recordPage = await this.LoginAndNavigateToRecordAsync(new RecordFaker().Generate());

            await recordPage.Form.GetField<ISingleLineText>(nameof(pp_Record.pp_singlelineoftexttext)).Control.SetValueAsync("Invoke process error");
            await recordPage.Form.CommandBar.ClickCommandAsync("Save");

            return recordPage.ErrorDialog;
        }
    }
}
