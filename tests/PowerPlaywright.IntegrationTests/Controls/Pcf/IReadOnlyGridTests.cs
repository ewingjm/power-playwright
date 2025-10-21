namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Text.RegularExpressions;
    using Bogus;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IReadOnlyGrid"/> control class.
    /// </summary>
    public partial class IReadOnlyGridTests : IntegrationTests
    {
        private static readonly string[] Columns = ["Name", "Created On", "Created By", "Modified By", "Modified On", "Owner", "Record", "Status", "Status Reason", "Created By (Delegate)", "Modified By (Delegate)", "Owning Business Unit", "Record Created On"];

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
        /// Tests that <see cref="IReadOnlyGrid.OpenRecordAsync(int)"/> opens the record when called with an index that is in range.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenRecordAsync_IndexInRange_OpensRecord()
        {
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: [new RelatedRecordFaker()]);

            var recordPage = await gridControl.OpenRecordAsync(0);

            await this.Expect(recordPage.Page).ToHaveURLAsync(RelatedRecordFormUrlRegex());
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.OpenRecordAsync(int)"/> throws a <see cref="IndexOutOfRangeException"/> when the index is out of range.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenRecordAsync_IndexOutOfRange_ThrowsIndexOutOfRangeException()
        {
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: null);

            Assert.ThrowsAsync<IndexOutOfRangeException>(() => gridControl.OpenRecordAsync(1));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.GetColumnNamesAsync"/> always returns all column names.
        /// </summary>
        /// <param name="withRelatedRecords">Whether or not to create related records.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetColumnNamesAsync_Always_ReturnsAllColumnNamesInOrder(bool withRelatedRecords)
        {
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: withRelatedRecords ? [new RelatedRecordFaker()] : null);

            Assert.That(gridControl.GetColumnNamesAsync, Is.EqualTo(Columns));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.GetTotalRowCountAsync"/> always returns the total number of rows.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetTotalRowCountAsync_Always_ReturnsTotalRowCount()
        {
            var expectedTotalRowCount = this.faker.Random.Int(0, 2);
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            var rowCount = await gridControl.GetTotalRowCountAsync();

            Assert.That(rowCount, Is.EqualTo(expectedTotalRowCount));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.ToggleAllRowsAsync"/> sets the expected state to checked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleAllRows_NoRowsSelected_SelectsAllRows()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(expectedTotalRowCount));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.ToggleAllRowsAsync"/> sets the expected state to unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleAllRows_RowsSelected_DeselectsAllRows()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);
            await gridControl.ToggleSelectAllRowsAsync(select: false);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.ToggleAllRowsAsync"/> does not change any row states when there are no rows in the grid.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleAllRows_EmptyResultSet_SelectsNoRows()
        {
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Empty<RelatedRecordFaker>());

            await gridControl.ToggleSelectAllRowsAsync(select: true);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.Zero);
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.GetSelectedRowCountAsync"/> returns the number of selected rows.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetSelectedRowCount_RowsSelected_ReturnsCountOfSelectedRows()
        {
            var expectedTotalRowCount = this.faker.Random.Int(1, 4);
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);
            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(expectedTotalRowCount));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.GetRowsAsync"/> returns row data from the currently visible page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetRowsAsync_HasRows_ReturnsRowData()
        {
            var expectedRowCount = 2;
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedRowCount).Select(i => new RelatedRecordFaker()));

            var rows = await gridControl.GetRowsAsync();
            var rowList = rows.ToList();

            Assert.That(rowList, Has.Count.EqualTo(expectedRowCount));
            Assert.That(rowList[0], Has.Count.GreaterThan(0));
            Assert.That(rowList[0].Keys, Does.Contain("Name"));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.GetRowsAsync"/> returns empty collection when grid is empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetRowsAsync_EmptyGrid_ReturnsEmptyCollection()
        {
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Empty<RelatedRecordFaker>());

            var rows = await gridControl.GetRowsAsync();

            Assert.That(rows, Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.ToggleSelectRowAsync"/> selects a specific row.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectRowAsync_SelectRow_SelectsSpecificRow()
        {
            var expectedRowCount = 3;
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectRowAsync(1, select: true);

            var selectedCount = await gridControl.GetSelectedRowCountAsync();
            Assert.That(selectedCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.ToggleSelectRowAsync"/> deselects a specific row.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectRowAsync_DeselectRow_DeselectsSpecificRow()
        {
            var expectedRowCount = 3;
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);
            await gridControl.ToggleSelectRowAsync(1, select: false);

            var selectedCount = await gridControl.GetSelectedRowCountAsync();
            Assert.That(selectedCount, Is.EqualTo(expectedRowCount - 1));
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.ToggleSelectRowAsync"/> throws when index is out of range.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectRowAsync_IndexOutOfRange_ThrowsIndexOutOfRangeException()
        {
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, 2).Select(i => new RelatedRecordFaker()));

            Assert.ThrowsAsync<IndexOutOfRangeException>(() => gridControl.ToggleSelectRowAsync(5, select: true));
        }

        [GeneratedRegex(".*pagetype=entityrecord&etn=pp_relatedrecord.*")]
        private static partial Regex RelatedRecordFormUrlRegex();

        private async Task<IReadOnlyGrid> SetupReadOnlyGridScenarioAsync(Faker<pp_Record>? withRecord = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatedRecords = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatableRecords = null)
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

            return recordPage.Form.GetDataSet(pp_Record.Forms.Information.RelatedRecordsSubgrid).GetControl<IReadOnlyGrid>();
        }
    }
}