namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="ICommandBar"/> control.
    /// </summary>
    public class ICommandBarTests : IntegrationTests
    {
        private static readonly string[] CustomCommands = ["Command", "Dropdown", "Split Button"];
        private static readonly string[] OutOfTheBoxCommands = ["Save", "Save & Close", "New", "Deactivate", "Delete", "Refresh", "Check Access", "Assign", "Flow", "Run Report"];
        private static readonly string[] OutOfTheBoxFlowCommands = ["Create a flow", "See your flows"];

        /// <summary>
        /// Tests that the <see cref="ICommandBar.GetCommandsAsync"/> method returns the labels of all commands when no parent commands are passed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetCommandsAsync_NoParentCommandsPassed_ReturnsLabelsOfAllCommands()
        {
            var commandBar = (await this.SetupCommandBarScenarioAsync()).Form.CommandBar;

            var commands = await commandBar.GetCommandsAsync();

            CollectionAssert.IsNotEmpty(commands);
            CollectionAssert.DoesNotContain(commands, string.Empty);
            CollectionAssert.IsSupersetOf(commands, CustomCommands.Concat(OutOfTheBoxCommands));
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.GetCommandsAsync"/> method returns the labels of all commands under the parent command when parent commands are passed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetCommandsAsync_ParentCommandsPassed_ReturnsLabelsOfAllCommandsUnderParentCommands()
        {
            var commandBar = (await this.SetupCommandBarScenarioAsync()).Form.CommandBar;

            var commands = await commandBar.GetCommandsAsync("Dropdown", "Child Dropdown");

            CollectionAssert.AreEquivalent(new string[] { "Grandchild Command" }, commands);
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.GetCommandsAsync"/> method returns the labels of all commands under the parent command when parent commands are passed and are in the overflow.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetCommandsAsync_ParentCommandsPassedAndRootCommandInOverflow_ReturnsLabelsOfAllCommandsUnderParentCommands()
        {
            var commandBar = (await this.SetupCommandBarScenarioAsync()).Form.CommandBar;

            var commands = await commandBar.GetCommandsAsync("Flow");

            CollectionAssert.AreEquivalent(OutOfTheBoxFlowCommands, commands);
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync"/> method clicks a command when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ClickCommandAsync_CommandVisible_ClicksCommand()
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = page.Form.CommandBar;

            await commandBar.ClickCommandAsync("Split Button");

            Assert.That(page.ConfirmDialog.GetTextAsync, Is.EqualTo("You clicked 'Split Button'."));
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync"/> method clicks a command when it is in the overflow menu.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ClickCommandAsync_CommandInOverflow_ClicksCommand()
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = page.Form.CommandBar;

            await commandBar.ClickCommandAsync("Delete");

            Assert.That(page.ConfirmDialog.GetTitleAsync, Is.EqualTo("Confirm Deletion"));
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync"/> method clicks a command when parent commands are passed and the command is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ClickCommandAsync_ParentCommandsPassed_ClicksCommand()
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = page.Form.CommandBar;

            await commandBar.ClickCommandAsync("Dropdown", "Child Dropdown", "Grandchild Command");

            Assert.That(page.ConfirmDialog.GetTextAsync, Is.EqualTo("You clicked 'Dropdown -> Child Dropdown -> Grandchild Command'."));
        }

        /// <summary>
        /// Sets up a command bar scenario.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<IEntityRecordPage> SetupCommandBarScenarioAsync()
        {
            return await this.LoginAndNavigateToRecordAsync(new RecordFaker().Generate());
        }
    }
}
