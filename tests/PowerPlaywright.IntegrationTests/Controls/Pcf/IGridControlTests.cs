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
    /// Tests for the <see cref="IEditableGrid"/> control class.
    /// </summary>
    public partial class IGridControlTests : IntegrationTests
    {
        private static readonly string[] Columns = ["Name", "Created On", "Created By", "Modified By", "Modified On", "Owner", "Record", "Status", "Status Reason", "Created By (Delegate)", "Modified By (Delegate)", "Owning Business Unit", "Record Created On"];
        private static readonly string[] EditableColumns = ["Name", "Record"];

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
        /// Tests that <see cref="IEditableGrid.GetSelectedRowCountAsync"/> returns the number of selected rows.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetSelectedRowCountAsync_RowsSelected_ReturnsCountOfSelectedRows()
        {
            var expectedTotalRowCount = this.faker.Random.Int(1, 4);
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);
            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(expectedTotalRowCount));
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.OpenRecordAsync(int)"/> opens the record when called with an index that is in range.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenRecordAsync_IndexInRange_OpensRecord()
        {
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: [new RelatedRecordFaker()]);

            var recordPage = await gridControl.OpenRecordAsync(0);

            await this.Expect(recordPage.Page).ToHaveURLAsync(RelatedRecordFormUrlRegex());
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.OpenRecordAsync(int)"/> throws a <see cref="IndexOutOfRangeException"/> when the index is out of range.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenRecordAsync_IndexOutOfRange_ThrowsIndexOutOfRangeException()
        {
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: null);

            Assert.ThrowsAsync<IndexOutOfRangeException>(() => gridControl.OpenRecordAsync(1));
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.GetColumnNamesAsync"/> always returns all column names.
        /// </summary>
        /// <param name="withRelatedRecords">Whether or not to create related records.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestCase(true)]
        [TestCase(false)]
        public async Task GetColumnNamesAsync_Always_ReturnsAllColumnNamesInOrder(bool withRelatedRecords)
        {
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: withRelatedRecords ? [new RelatedRecordFaker()] : null);

            Assert.That(gridControl.GetColumnNamesAsync, Is.EqualTo(Columns));
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.ToggleSelectRowAsync"/> sets the expected state to unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectRowAsync_NoRowsSelected_SelectRow()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectRowAsync(2, select: true);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.ToggleSelectRowAsync"/> sets the expected state to unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectRowAsync_RowsSelected_DeselectRow()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectRowAsync(3, select: true);
            await gridControl.ToggleSelectRowAsync(3, select: false);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.ToggleSelectAllRowsAsync"/> does not change any row states when there are no rows in the grid.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectAllRowsAsync_EmptyResultSet_SelectsNoRows()
        {
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: Enumerable.Empty<RelatedRecordFaker>());

            await gridControl.ToggleSelectAllRowsAsync(select: true);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.Zero);
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.ToggleSelectAllRowsAsync"/> sets the expected state to checked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectAllRowsAsync_NoRowsSelected_SelectsAllRows()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(expectedTotalRowCount));
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.ToggleSelectAllRowsAsync"/> sets the expected state to unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleSelectAllRowsAsync_RowsSelected_DeselectsAllRows()
        {
            var expectedTotalRowCount = 4;
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(select: true);
            await gridControl.ToggleSelectAllRowsAsync(select: false);

            var actualSelectedRowCount = await gridControl.GetSelectedRowCountAsync();

            Assert.That(actualSelectedRowCount, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.GetEditableColumnsAsync"/> always returns the editable columns.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetEditableColumnsAsync_Always_ReturnsAllColumnNamesInOrder()
        {
            var expectedTotalRowCount = 1;
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            Assert.That(await gridControl.GetEditableColumnsAsync(1), Is.EqualTo(EditableColumns));
        }

        /// <summary>
        /// Tests that <see cref="IEditableGrid.GetErrorNotificationsAsync"/> returns error notifications.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetErrorNotificationsAsync_InvalidRows_ReturnsErrorNotifications()
        {
            var expectedTotalRowCount = 1;
            var gridControl = await this.SetupEditableGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.UpdateRowAsync(0, new Dictionary<string, string>
            {
                ["Name"] = string.Empty,
                ["Record"] = string.Empty,
            });

            var notifications = await gridControl.GetErrorNotificationsAsync();

            Assert.That(notifications, Has.Count.EqualTo(1));
        }

        [GeneratedRegex(".*pagetype=entityrecord&etn=pp_relatedrecord.*")]
        private static partial Regex RelatedRecordFormUrlRegex();

        private async Task<IEditableGrid> SetupEditableGridScenarioAsync(Faker<pp_Record>? withRecord = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatedRecords = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatableRecords = null)
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

            return recordPage.Form.GetDataSet(pp_Record.Forms.Information.RelatedEditableRecordsSubGrid).GetControl<IEditableGrid>();
        }
    }
}