namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="ICommandBar"/> control.
    /// </summary>
    public class ICommandBarControlTests : IntegrationTests
    {
        private ICommandBar commandBar;

        /// <summary>
        /// Sets up the command bar grid.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            var recordPage = await this.LoginAndNavigateToRecordAsync(new RecordFaker().Generate());

            this.commandBar = recordPage.CommandBar;
        }

        [Test]
        public async Task GetCommandsAsync_NoParentCommandsPassed_ReturnsLabelsOfVisibleCommands()
        {
            var commands = await this.commandBar.GetCommandsAsync();

            CollectionAssert.IsSubsetOf(commands, new string[] { "Save", "Save & Close", "New", "Deactivate", "Delete", "Refresh" });
        }
    }
}
