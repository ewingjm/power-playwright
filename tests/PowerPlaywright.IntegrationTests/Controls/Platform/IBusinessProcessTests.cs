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
        /// moves to the next stage and verifies that the stage has changed by comparing the current stage before and after the move. Resulting in true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task NextAsync_WhenAvailable_ReturnsTrue()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            var didChangeStage = await businessProcessFlow.NextAsync();

            Assert.That(didChangeStage, Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.MoveToNextStageAsync(string[])"/> method
        /// returns the same stage when already at the last stage. Resulting in false.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task NextAsync_WhenAtLastStage_ReturnsFalse()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            await this.MoveToFinalStageAsync(businessProcessFlow);

            Assert.That(await businessProcessFlow.NextAsync(), Is.False);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.PreviousAsync(string[])"/> method
        /// returns the same stage when already at the last stage. Resulting in false.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task PreviousAsync_AtFirstStage_ReturnsFalse()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            Assert.That(await businessProcessFlow.PreviousAsync(), Is.False);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.PreviousAsync(string[])"/> method
        /// returns the the previous stage after previously moving forward. Resulting in true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task PreviousAsync_WhenAvailable_ReturnsTrue()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            await businessProcessFlow.NextAsync();

            Assert.That(await businessProcessFlow.PreviousAsync(), Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.CompleteAsync(string[])"/> method
        /// returns a boolean to confirm completion.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CompleteAsync_WhenLastStage_ReturnsTrue()
        {
            var page = await this.SetupBusinessProcessFlowScenarioAsync();
            var businessProcessFlow = page.Form.BusinessProcess;

            await this.MoveToFinalStageAsync(businessProcessFlow);

            Assert.That(await businessProcessFlow.CompleteAsync(), Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="IBusinessProcessFlow.CompleteAsync(string[])"/> method
        /// throws an exception when called before reaching the final stage.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CompleteAsync_WhenNotLastStage_ThrowsException()
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

            await this.MoveToFinalStageAsync(businessProcessFlow);

            await businessProcessFlow.CompleteAsync();

            Assert.That(await businessProcessFlow.IsProcessCompleteAsync(), Is.True);
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

            Assert.That(await businessProcessFlow.IsProcessCompleteAsync(), Is.False);
        }

        /// <summary>
        /// Helper method to move the business process flow to the final stage.
        /// </summary>
        private async Task MoveToFinalStageAsync(IBusinessProcessFlow bpf)
        {
            for (var i = 0; i < ProcessStages.Length; i++)
            {
                if (await bpf.IsFinalStageAsync())
                {
                    break;
                }

                await bpf.NextAsync();
            }
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