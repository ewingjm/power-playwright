namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IBusinessProcessFlow"/> control.
    /// </summary>
    public class IBusinessProcessTests : IntegrationTests
    {
        private static readonly string[] ProcessStages = ["Details", "Processing", "Resolution"];

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.GetStagesAsync(string[])"/> method returns the labels of all the stages of the bpf.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetStagesAsync_ReturnsAllStages()
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
        public async Task MoveToNextStageAsync_ReturnsNewStage()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            var currentStage = await businessProcessFlow.GetCurrentStageAsync();

            string newStage = await businessProcessFlow.NextAsync();

            Assert.That(currentStage != newStage);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.MoveToNextStageAsync(string[])"/> method
        /// returns the same stage when already at the last stage.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task MoveToNextStageAsync_ReturnsSameStageWhenAlreadyAtLastStage()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            while (await businessProcessFlow.IsFinalStageAsync())
            {
                await businessProcessFlow.NextAsync();
            }

            var returnedStage = await businessProcessFlow.NextAsync();

            Assert.That(returnedStage, Is.EqualTo(ProcessStages[ProcessStages.Length - 1]));
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.CompleteAsync(string[])"/> method
        /// returns a boolean to confirm completion.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CompleteAsync_ReturnsCompleted()
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
        public async Task CompleteAsync_ThrowsExceptionWhenAtEarlierStage()
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
        public async Task IsProcessCompleteAsync_ReturnsTrue()
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
        public async Task IsProcessCompleteAsync_ReturnsFalse()
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