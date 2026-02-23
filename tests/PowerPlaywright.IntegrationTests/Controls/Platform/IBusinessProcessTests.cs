namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IBusinessProcessFlow"/> control.
    /// </summary>
    public class IBusinessProcessTests : IntegrationTests
    {
        private static readonly string[] ProcessStages = ["Validate", "Process", "Resolve"];

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.GetStagesAsync(string[])"/> method returns the labels of all the stages of the bpf.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetStagesAsync_BusinessProcessFlowVisible_ReturnsAllStages()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            var processStages = await businessProcessFlow.GetStagesAsync();
            CollectionAssert.IsSupersetOf(processStages, ProcessStages);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.MoveToNextStageAsync(string[])"/> method returns the new stage of the business process flow after moving.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task NextAsync_WhenPossible_ReturnsTrue()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            var didChangeStage = await businessProcessFlow.NextAsync();

            Assert.IsTrue(didChangeStage);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.MoveToNextStageAsync(string[])"/> method
        /// returns the same stage when already at the last stage.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task NextStageAsync_WhenAlreadyAtLastStage_ReturnsFalse()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            while (await businessProcessFlow.IsFinalStageAsync())
            {
                await businessProcessFlow.NextAsync();
            }

            var didChangeStage = await businessProcessFlow.NextAsync();

            Assert.IsFalse(didChangeStage);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.CompleteAsync(string[])"/> method
        /// returns a boolean to confirm completion.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CompleteAsync_WhenAtLastStage_ReturnsTrue()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            while (await businessProcessFlow.IsFinalStageAsync())
            {
                await businessProcessFlow.NextAsync();
            }

            var didComplete = await businessProcessFlow.CompleteAsync();

            Assert.IsTrue(didComplete);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.CompleteAsync(string[])"/> method
        /// throws an exception when called before reaching the final stage.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CompleteAsync_WhenNotAtLastStage_ThrowsException()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            Assert.ThrowsAsync<PowerPlaywrightException>(
            () => businessProcessFlow.CompleteAsync());
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.IsProcessCompleteAsync(string[])"/> method
        /// returns a boolean to confirm completion.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsProcessCompleteAsync_WhenComplete_ReturnsTrue()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            while (await businessProcessFlow.IsFinalStageAsync())
            {
                await businessProcessFlow.NextAsync();
            }

            await businessProcessFlow.CompleteAsync();

            var isComplete = await businessProcessFlow.IsProcessCompleteAsync();

            Assert.IsTrue(isComplete);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.IsProcessCompleteAsync(string[])"/> method
        /// returns a boolean to confirm completion.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsProcessCompleteAsync_WhenNotComplete_ReturnsFalse()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            var isComplete = await businessProcessFlow.IsProcessCompleteAsync();

            Assert.IsFalse(isComplete);
        }

        /// <summary>
        /// Sets up a business process.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<IEntityRecordPage> SetupBusinessProcessFlowScenarioAsync()
        {
            return await this.LoginAndNavigateToRecordAsync(new RecordFaker().Generate());
        }
    }
}