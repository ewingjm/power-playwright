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
        /// <param name="pageType">The type of form.</param>
        /// <param name="viewName">Expected view name.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IEntityRecordPage), pp_RelatedRecord.Views.InactiveRelatedRecords)]
        [TestCase(typeof(IEntityListPage), pp_Record.Views.InactiveRecords)]
        public async Task SwitchViewAsync_ViewSelectorVisibleAndViewExists_SwitchesView(Type pageType, string viewName)
        {
            var dataSet = await this.SetupDataSetScenarioAsync(pageType);

            await dataSet.SwitchViewAsync(viewName);

            Assert.That(dataSet.GetActiveViewAsync, Is.EqualTo(viewName));
        }

        /// <summary>
        /// Tests that calling <see cref="IDataSet.SwitchViewAsync"/> throws a <see cref="PowerPlaywrightException"/> when the view selector is visible but the view doesn't exists.
        /// </summary>
        /// <param name="pageType">The type of form.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IEntityRecordPage))]
        [TestCase(typeof(IEntityListPage))]
        public async Task SwitchViewAsync_ViewSelectorVisibleButViewDoesNotExist_ThrowsPowerPlaywrightException(Type pageType)
        {
            var dataSet = await this.SetupDataSetScenarioAsync(pageType);

            Assert.ThrowsAsync<PowerPlaywrightException>(() => dataSet.SwitchViewAsync("A view that doesn't exist"));
        }

        /// <summary>
        /// Tests that calling <see cref="IDataSet.GetActiveViewAsync"/> returns the active view when the view selector is visible.
        /// </summary>
        /// <param name="pageType">The type of form.</param>
        /// <param name="viewName">Expected view name.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IEntityRecordPage), pp_RelatedRecord.Views.ActiveRelatedRecords)]
        [TestCase(typeof(IEntityListPage), pp_Record.Views.ActiveRecords)]
        public async Task GetActiveViewAsync_ViewSelectorVisible_ReturnsActiveView(Type pageType, string viewName)
        {
            var dataSet = await this.SetupDataSetScenarioAsync(pageType);

            Assert.That(dataSet.GetActiveViewAsync, Is.EqualTo(viewName));
        }

        private async Task<IDataSet> SetupDataSetScenarioAsync(Type pageType)
        {
            return pageType == typeof(IEntityRecordPage)
                ? await this.SetupDataSetScenarioRecordPage()
                : await this.SetupDataSetScenarioListPage();
        }

        private async Task<IDataSet> SetupDataSetScenarioRecordPage(Faker<pp_Record>? withRecord = null)
        {
            withRecord ??= new RecordFaker();

            var recordPage = await this.LoginAndNavigateToRecordAsync(withRecord.Generate());

            return recordPage.Form.GetDataSet(pp_Record.Forms.Information.RelatedRecordsSubgrid);
        }

        private async Task<IDataSet> SetupDataSetScenarioListPage(IEnumerable<Faker<pp_Record>>? withRecords = null)
        {
            withRecords ??= [new RecordFaker()];

            await Task.WhenAll(withRecords.Select(r => this.CreateRecordAsync(r.Generate())));

            var entityListPage = await this.LoginAsync();

            return ((IEntityListPage)entityListPage).DataSet;
        }
    }
}