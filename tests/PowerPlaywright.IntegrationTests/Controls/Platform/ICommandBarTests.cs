namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="ICommandBar"/> control.
    /// </summary>
    public class ICommandBarTests : IntegrationTests
    {
        private static readonly string[] OutOfTheBoxCommands = ["Save", "Save & Close", "New", "Deactivate", "Delete", "Refresh", "Check Access", "Assign", "Flow", "Word Templates", "Run Report"];
        private static readonly string[] OutOfTheBoxFlowCommands = ["Create a flow", "See your flows"];

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

        /// <summary>
        /// Tests that the <see cref="ICommandBar.GetCommandsAsync"/> method returns the labels of all commands when no parent commands are passed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetCommandsAsync_NoParentCommandsPassed_ReturnsLabelsOfAllCommands()
        {
            var commands = await this.commandBar.GetCommandsAsync();

            CollectionAssert.IsNotEmpty(commands);
            CollectionAssert.DoesNotContain(commands, string.Empty);
            CollectionAssert.IsSubsetOf(commands, OutOfTheBoxCommands);
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.GetCommandsAsync"/> method returns the labels of all commands under the parent command when parent commands are passed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetCommandsAsync_ParentCommandsPassed_ReturnsLabelsOfAllCommandsUnderParentCommands()
        {
            var commands = await this.commandBar.GetCommandsAsync("Flow");

            CollectionAssert.IsSubsetOf(commands, OutOfTheBoxFlowCommands);
        }
    }
}
