namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using Bogus;
    using Microsoft.Xrm.Sdk;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.IntegrationTests.Extensions;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.App;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IReadOnlyGrid"/> control class.
    /// </summary>
    public class IDataSetTests : IntegrationTests
    {
        private static readonly string[] SearchTerms = ["Megatron", "Optimus Prime", "Grimlock", "Ultra Magnus"];

        private readonly Faker faker;

        /// <summary>
        /// Initializes a new instance of the <see cref="IDataSetTests"/> class.
        /// </summary>
        public IDataSetTests()
        {
            this.faker = new Faker();
        }

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

        /// <summary>
        /// Tests that <see cref="IDataSet.SearchAsync"/> filters data rows by search term.
        /// </summary>
        /// <param name="pageType">The type of form.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IEntityRecordPage))]
        [TestCase(typeof(IEntityListPage))]
        public async Task SearchAsync_KnownSearchTerm_FilteredResultSet(Type pageType)
        {
            var dataSet = await this.SetupDataSetSearchScenarioAsync(pageType);

            var searchTerm = this.faker.PickRandom(SearchTerms);

            await dataSet.SearchAsync(searchTerm);
            var dataRows = await dataSet.GetControl<IReadOnlyGrid>().GetRowDataAsync();

            Assert.That(dataRows.First().Get("Name"), Is.EqualTo(searchTerm));
        }

        /// <summary>
        /// Tests that <see cref="IDataSet.SearchAsync"/> filters data rows by search term.
        /// </summary>
        /// <param name="pageType">The type of form.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IEntityRecordPage))]
        [TestCase(typeof(IEntityListPage))]
        public async Task SearchAsync_UnknownSearchTerm_EmptyResultSet(Type pageType)
        {
            var dataSet = await this.SetupDataSetSearchScenarioAsync(pageType);

            await dataSet.SearchAsync("Rodimus Prime");
            var rowData = await dataSet.GetControl<IReadOnlyGrid>().GetRowDataAsync();

            Assert.That(rowData.ToList(), Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="IDataSet.SearchAsync"/> throws a <see cref="ArgumentException"/> when the search term is empty.
        /// </summary>
        /// <param name="pageType">The type of form.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IEntityRecordPage))]
        [TestCase(typeof(IEntityListPage))]
        public async Task SearchAsync_EmptySearchTerm_ThrowsArgumentException(Type pageType)
        {
            var dataSet = await this.SetupDataSetSearchScenarioAsync(pageType);

            Assert.ThrowsAsync<ArgumentException>(() => dataSet.SearchAsync(string.Empty));
        }

        /// <summary>
        /// Tests that <see cref="IDataSet.SearchAsync"/> throws a <see cref="ArgumentException"/> when the search term is null.
        /// </summary>
        /// <param name="pageType">The type of form.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(typeof(IEntityRecordPage))]
        [TestCase(typeof(IEntityListPage))]
        public async Task SearchAsync_NullSearchTerm_ThrowsArgumentException(Type pageType)
        {
            var dataSet = await this.SetupDataSetSearchScenarioAsync(pageType);

            Assert.ThrowsAsync<ArgumentException>(() => dataSet.SearchAsync(null));
        }

        private async Task<IDataSet> SetupDataSetSearchScenarioAsync(Type pageType)
        {
            EntityReference? withRecord = null;

            if (pageType == typeof(IEntityRecordPage))
            {
                var gridRecords = SearchTerms.Select(name =>
                        new RelatedRecordFaker()
                            .RuleFor(r => r.pp_Name, f => name)
                            .Generate());

                var record = new RecordFaker()
                    .RuleFor(r => r.pp_Record_RelatedRecord, (f, r) => gridRecords);

                withRecord = await this.CreateRecordAsync(record.Generate());
            }
            else if (pageType == typeof(IEntityListPage))
            {
                var gridRecords = SearchTerms.Select(name =>
                    new RelatedRecordFaker()
                        .RuleFor(r => r.pp_Name, f => name)
                        .RuleFor(r => r.pp_RelatedRecordId, (f, r) => name.ToDeterministicGuid())
                        .Generate());

                await this.UpsertRecordsAsync([.. gridRecords]);
            }

            return await this.SetupDataSetScenarioAsync(pageType, withRecord);
        }

        private async Task<IDataSet> SetupDataSetScenarioAsync(Type pageType, EntityReference? withRecord = null)
        {
            return pageType == typeof(IEntityRecordPage)
                ? await this.SetupDataSetScenarioRecordPage(withRecord)
                : await this.SetupDataSetScenarioListPage();
        }

        private async Task<IDataSet> SetupDataSetScenarioRecordPage(EntityReference? withRecord = null)
        {
            withRecord ??= await this.CreateRecordAsync(new RecordFaker().Generate());

            var recordPage = await this.LoginAndNavigateToRecordAsync(withRecord);

            return recordPage.Form.GetDataSet(pp_Record.Forms.Information.RelatedRecordsSubgrid);
        }

        private async Task<IDataSet> SetupDataSetScenarioListPage(IEnumerable<Faker<pp_RelatedRecord>>? withGridRecords = null)
        {
            withGridRecords ??= [new RelatedRecordFaker()];

            await Task.WhenAll(withGridRecords.Select(r => this.CreateRecordAsync(r.Generate())));

            var homePage = await this.LoginAsync();
            var listPage = await homePage.SiteMap.OpenPageAsync<IEntityListPage>(UserInterfaceDemo.SiteMap.AreaA.DisplayName, UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName, UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords);

            return listPage.DataSet;
        }
    }
}