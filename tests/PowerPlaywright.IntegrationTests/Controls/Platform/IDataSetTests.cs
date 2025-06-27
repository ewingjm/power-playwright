namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using Bogus;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IReadOnlyGrid"/> control class.
    /// </summary>
    public class IDataSetTests : IntegrationTests
    {
        /// <summary>
        /// Tests that calling <see cref="IDataSet.SwitchViewAsync"/> switches the active view when the view selector is visible and the view exists.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SwitchViewAsync_ViewSelectorVisibleAndViewExists_SwitchesView()
        {
            var dataSet = await this.SetupDataSetScenarioAsync();

            await dataSet.SwitchViewAsync(pp_Record.Views.InactiveRecords);

            Assert.That(dataSet.GetActiveViewAsync, Is.EqualTo(pp_Record.Views.InactiveRecords));
        }

        /// <summary>
        /// Tests that calling <see cref="IDataSet.SwitchViewAsync"/> throws a <see cref="PowerPlaywrightException"/> when the view selector is visible but the view doesn't exists.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SwitchViewAsync_ViewSelectorVisibleButViewDoesNotExist_ThrowsPowerPlaywrightException()
        {
            var dataSet = await this.SetupDataSetScenarioAsync();

            Assert.ThrowsAsync<PowerPlaywrightException>(() => dataSet.SwitchViewAsync("A view that doesn't exist"));
        }

        /// <summary>
        /// Tests that calling <see cref="IDataSet.GetActiveViewAsync"/> returns the active view when the view selector is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetActiveViewAsync_ViewSelectorVisible_ReturnsActiveView()
        {
            var dataSet = await this.SetupDataSetScenarioAsync();

            Assert.That(dataSet.GetActiveViewAsync, Is.EqualTo(pp_Record.Views.ActiveRecords));
        }

        private async Task<IDataSet> SetupDataSetScenarioAsync(IEnumerable<Faker<pp_Record>>? withRecords = null)
        {
            withRecords ??= [new RecordFaker()];

            await Task.WhenAll(withRecords.Select(r => this.CreateRecordAsync(r.Generate())));

            var entityListPage = await this.LoginAsync();

            return ((IEntityListPage)entityListPage).DataSet;
        }
    }
}