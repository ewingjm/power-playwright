namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="ICommandBar"/> control.
    /// </summary>
    public class ICommandBarTests : IntegrationTests
    {
        private static readonly string[] CustomCommands = ["Command", "Dropdown", "Split Button"];
        private static readonly string[] OutOfTheBoxMainFormCommands = ["Save", "Save & Close", "New", "Deactivate", "Delete", "Refresh", "Check Access", "Assign", "Flow", "Run Report"];
        private static readonly string[] OutOfTheBoxFlowCommands = ["Create a flow", "See your flows"];
        private static readonly string[] OutOfTheBoxSubgridCommands = ["New Related Record", "Add Existing Related Record", "Refresh", "Flow", "Run Report", "See associated records"];

        /// <summary>
        /// Tests that the <see cref="ICommandBar.GetCommandsAsync(string[])"/> method returns the labels of all commands when no parent commands are passed.
        /// </summary>
        /// <param name="parentType">The type of parent control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IMainForm))]
        [TestCase(typeof(IDataSet))]
        public async Task GetCommandsAsync_NoParentCommandsPassed_ReturnsLabelsOfAllCommands(Type parentType)
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = this.GetCommandBarByParentType(parentType, page);

            var commands = await commandBar.GetCommandsAsync();

            CollectionAssert.IsNotEmpty(commands);
            CollectionAssert.DoesNotContain(commands, string.Empty);
            CollectionAssert.IsSupersetOf(commands, parentType == typeof(IMainForm) ? CustomCommands.Concat(OutOfTheBoxMainFormCommands) : OutOfTheBoxSubgridCommands);
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.GetCommandsAsync(string[])"/> method returns the labels of all commands under the parent command when parent commands are passed.
        /// </summary>
        /// <param name="parentType">The type of parent control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IMainForm))]
        [TestCase(typeof(IDataSet))]
        public async Task GetCommandsAsync_ParentCommandsPassed_ReturnsLabelsOfAllCommandsUnderParentCommands(Type parentType)
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = this.GetCommandBarByParentType(parentType, page);

            var commands = await commandBar.GetCommandsAsync("Dropdown", "Child Dropdown");

            CollectionAssert.AreEquivalent(new string[] { "Grandchild Command" }, commands);
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync(string[])"/> method returns the labels of all commands under the parent command when parent commands are passed and are in the overflow.
        /// </summary>
        /// <param name="parentType">The type of parent control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IMainForm))]
        [TestCase(typeof(IDataSet))]
        public async Task GetCommandsAsync_ParentCommandsPassedAndRootCommandInOverflow_ReturnsLabelsOfAllCommandsUnderParentCommands(Type parentType)
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = this.GetCommandBarByParentType(parentType, page);

            var commands = await commandBar.GetCommandsAsync("Flow");

            CollectionAssert.AreEquivalent(OutOfTheBoxFlowCommands, commands);
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync(string[])"/> method clicks a command when it is visible.
        /// </summary>
        /// <param name="parentType">The type of parent control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IMainForm))]
        [TestCase(typeof(IDataSet))]
        public async Task ClickCommandAsync_CommandVisible_ClicksCommand(Type parentType)
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = this.GetCommandBarByParentType(parentType, page);

            await commandBar.ClickCommandAsync("Split Button");

            Assert.That(page.ConfirmDialog.GetTextAsync, Is.EqualTo($"You clicked 'Split Button'{(parentType == typeof(IDataSet) ? " on the related record" : string.Empty)}."));
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync(string[])"/> method clicks a command when it is in the overflow menu.
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
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync(string[])"/> method clicks a command when parent commands are passed and the command is visible.
        /// </summary>
        /// <param name="parentType">The type of parent control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IMainForm))]
        [TestCase(typeof(IDataSet))]
        public async Task ClickCommandAsync_ParentCommandsPassed_ClicksCommand(Type parentType)
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = this.GetCommandBarByParentType(parentType, page);

            await commandBar.ClickCommandAsync("Dropdown", "Child Dropdown", "Grandchild Command");

            Assert.That(page.ConfirmDialog.GetTextAsync, Is.EqualTo($"You clicked 'Dropdown -> Child Dropdown -> Grandchild Command'{(parentType == typeof(IDataSet) ? " on the related record" : string.Empty)}."));
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync{TModelDrivenAppPage}(string[])"/> method clicks a command when parent commands are passed and the command is visible.
        /// </summary>
        /// <param name="parentType">The type of parent control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ClickCommandAsync_PageTypeParameterPassed_ClicksCommandAndReturnsRequestedPage()
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = page.Form.CommandBar;

            var listPage = await commandBar.ClickCommandAsync<IEntityListPage>("Save & Close");

            Assert.That(await listPage.DataSet.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="ICommandBar.ClickCommandAsync{TModelDrivenAppPage}(string[])"/> method clicks a command when parent commands are passed and the command is visible.
        /// </summary>
        /// <param name="parentType">The type of parent control.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ClickCommandWithDialogAsync_ControlTypePassed_ReturnsInstanceOfControl()
        {
            var page = await this.SetupCommandBarScenarioAsync();
            var commandBar = page.Form.CommandBar;

            var lookupDialog = await commandBar.ClickCommandWithDialogAsync<ILookupDialog>("Add Existing Related Record");

            Assert.That(await lookupDialog.IsVisibleAsync(), Is.True);
        }

        private ICommandBar GetCommandBarByParentType(Type parentType, IEntityRecordPage page)
        {
            return parentType == typeof(IMainForm)
                ? page.Form.CommandBar
                : page.Form.GetDataSet(pp_Record.Forms.Information.RelatedRecordsSubgrid).CommandBar;
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
