namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using Bogus;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IGridControl"/> control class.
    /// </summary>
    public partial class IGridControlTests : IntegrationTests
    {
        private static readonly string[] Columns = ["Name", "Created On", "Created By", "Modified By", "Modified On", "Owner", "Record", "Status", "Status Reason", "Created By (Delegate)", "Modified By (Delegate)", "Owning Business Unit", "Record Created On"];
        private static readonly string[] EditableColumns = ["Name", "Record"];
        private static readonly string[] SearchTerms = ["Megatron", "Optimus Prime", "Grimlock", "Ultra Magnus"];

        private Faker faker;

        /// <summary>
        /// Sets up the lookup control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.GetSelectedRowCountAsync"/> returns the number of selected rows.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetSelectedRowCountAsync_RowsSelected_ReturnsCountOfSelectedRows()
        {
            var expectedTotalRowCount = this.faker.Random.Int(1, 4);
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);
            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(expectedTotalRowCount));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.OpenRecordAsync(int)"/> opens the record when called with an index that is in range.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenRecordAsync_IndexInRange_OpensRecord()
        {
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: [new RelatedRecordFaker()]);

            var recordPage = await gridControl.OpenRecordAsync(0);

            await this.Expect(recordPage.Page).ToHaveURLAsync(RelatedRecordFormUrlRegex());
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.OpenRecordAsync(int)"/> throws a <see cref="IndexOutOfRangeException"/> when the index is out of range.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenRecordAsync_IndexOutOfRange_ThrowsIndexOutOfRangeException()
        {
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: null);

            Assert.ThrowsAsync<IndexOutOfRangeException>(() => gridControl.OpenRecordAsync(1));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.GetColumnNamesAsync"/> always returns all column names.
        /// </summary>
        /// <param name="withRelatedRecords">Whether or not to create related records.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetColumnNamesAsync_Always_ReturnsAllColumnNamesInOrder(bool withRelatedRecords)
        {
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: withRelatedRecords ? [new RelatedRecordFaker()] : null);

            Assert.That(gridControl.GetColumnNamesAsync, Is.EqualTo(Columns));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.ToggleSelectRowAsync"/> sets the expected state to unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectRowAsync_NoRowsSelected_SelectRow()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectRowAsync(2, select: true);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.ToggleSelectRowAsync"/> sets the expected state to unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectRowAsync_RowsSelected_DeselectRow()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectRowAsync(3, select: true);
            await gridControl.ToggleSelectRowAsync(3, select: false);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.ToggleSelectAllRowsAsync"/> does not change any row states when there are no rows in the grid.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectAllRowsAsync_EmptyResultSet_SelectsNoRows()
        {
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Empty<RelatedRecordFaker>());

            await gridControl.ToggleSelectAllRowsAsync(select: true);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.Zero);
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.ToggleSelectAllRowsAsync"/> sets the expected state to checked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectAllRowsAsync_NoRowsSelected_SelectsAllRows()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(expectedTotalRowCount));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.ToggleSelectAllRowsAsync"/> sets the expected state to unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectAllRowsAsync_RowsSelected_DeselectsAllRows()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);
            await gridControl.ToggleSelectAllRowsAsync(select: false);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.GetEditableColumnsAsync"/> always returns the editable columns.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetEditableColumnsAsync_Always_ReturnsAllColumnNamesInOrder()
        {
            var expectedTotalRowCount = 2;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            Assert.That(await gridControl.GetEditableColumnsAsync(0), Is.EqualTo(EditableColumns));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.GetToggledStateAsync"/> always returns the editable columns.
        /// </summary>
        /// <param name="selectedState">Default checkbox state.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetToggledStateAsync_Always_ReturnsTheCheckBoxState(bool selectedState)
        {
            var expectedTotalRowCount = 1;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectRowAsync(0, select: selectedState);

            Assert.That(await gridControl.GetToggledStateAsync(0), Is.EqualTo(selectedState));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.GetErrorNotificationsAsync"/> returns error notifications.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetErrorNotificationsAsync_InvalidRows_ReturnsErrorNotifications()
        {
            var expectedTotalRowCount = 1;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.UpdateRowAsync(0, new Dictionary<string, string>
            {
                ["Name"] = string.Empty,
                ["Record"] = string.Empty,
            });

            var notifications = await gridControl.GetErrorNotificationsAsync();

            Assert.That(notifications, Has.Count.EqualTo(1));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.UpdateRowAsync"/> updates editable cells within a row.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task UpdateRowAsync_EditableRow_UpdatesRow()
        {
            var expectedTotalRowCount = 1;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));
            var name = string.Join(" ", this.faker.Lorem.Words(8));

            await gridControl.UpdateRowAsync(0, new Dictionary<string, string>
            {
                ["Name"] = name,
            });

            var dataRows = await gridControl.GetRowDataAsync();

            Assert.That(dataRows.ToList(), Has.Count.EqualTo(1));
            Assert.That(dataRows.Select(rd => rd["Name"]).First(), Is.EqualTo(name));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.SearchAsync"/> filters data rows by search term.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_KnownSearchTerm_FilteredResultSet()
        {
            var gridControl = await this.SetupReadOnlyGridSearchScenarioAsync();
            var searchTerm = this.faker.PickRandom(SearchTerms);

            await gridControl.SearchAsync(searchTerm);
            var dataRows = await gridControl.GetRowDataAsync();

            Assert.That(dataRows.ToList(), Has.Count.EqualTo(1));
            Assert.That(dataRows.First().Get("Name"), Is.EqualTo(searchTerm));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.SearchAsync"/> filters data rows by search term.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_UnknownSearchTerm_EmptyResultSet()
        {
            var gridControl = await this.SetupReadOnlyGridSearchScenarioAsync();

            await gridControl.SearchAsync("Rodimus Prime");
            var dataRows = await gridControl.GetRowDataAsync();

            Assert.That(dataRows.ToList(), Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.SearchAsync"/> throws a <see cref="ArgumentException"/> when the search term is empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_EmptySearchTerm_ThrowsArgumentException()
        {
            var gridControl = await this.SetupReadOnlyGridSearchScenarioAsync();

            Assert.ThrowsAsync<ArgumentException>(() => gridControl.SearchAsync(string.Empty));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.SearchAsync"/> throws a <see cref="ArgumentException"/> when the search term is null.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_NullSearchTerm_ThrowsArgumentException()
        {
            var gridControl = await this.SetupReadOnlyGridSearchScenarioAsync();

            Assert.ThrowsAsync<ArgumentException>(() => gridControl.SearchAsync(null));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.GetRowsAsync"/> returns row data from the currently visible page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetRowDataAsync_HasRows_ReturnsRowData()
        {
            var expectedRowCount = 2;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedRowCount).Select(i => new RelatedRecordFaker()));

            var dataRows = await gridControl.GetRowDataAsync();

            Assert.That(dataRows.ToList(), Has.Count.EqualTo(expectedRowCount));
            Assert.That(dataRows.First().Count(), Is.EqualTo(Columns.Length));
            Assert.That(dataRows.First().Select(c => c.Key), Is.EquivalentTo(Columns));
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.GetRowsAsync"/> returns empty collection when grid is empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetRowDataAsync_EmptyGrid_ReturnsEmptyCollection()
        {
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Empty<RelatedRecordFaker>());

            var dataRows = await gridControl.GetRowDataAsync();

            Assert.That(dataRows, Is.Empty);
        }

        [GeneratedRegex(".*pagetype=entityrecord&etn=pp_relatedrecord.*")]
        private static partial Regex RelatedRecordFormUrlRegex();

        private async Task<IGridControl> SetupGridControlScenarioAsync(Faker<pp_Record>? withRecord = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatedRecords = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatableRecords = null)
        {
            withRecord ??= new RecordFaker();

            if (withRelatedRecords != null && withRelatedRecords.Any())
            {
                withRecord.RuleFor(r => r.pp_Record_RelatedRecord, f => withRelatedRecords?.Select(f => f.Generate()));
            }

            if (withRelatableRecords != null)
            {
                using var client = this.GetServiceClient();

                await client.ExecuteAsync(
                    new CreateMultipleRequest
                    {
                        Targets = new EntityCollection([.. withRelatableRecords.Select(f => f.Generate())]),
                    });
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(withRecord.Generate());

            return recordPage.Form.GetDataSet(pp_Record.Forms.Information.RelatedEditableRecordsSubGrid).GetControl<IGridControl>();
        }

        private async Task<IGridControl> SetupReadOnlyGridSearchScenarioAsync()
        {
            var relatedRecords = SearchTerms.Select((name, index) =>
                new RelatedRecordFaker()
                    .RuleFor(r => r.pp_Name, f => name)).AsEnumerable();

            return await this.SetupGridControlScenarioAsync(withRelatedRecords: relatedRecords);
        }
    }
}