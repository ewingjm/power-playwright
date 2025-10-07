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
        /// Tests that <see cref="IReadOnlyGrid.ToggleAllRowsAsync"/> sets the expected state.
        /// </summary>
        /// <param name="expectedState">The expected checkbox state.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task ToggleAllRows_Always_SetsExpectedStateForAllRows(bool expectedState)
        {
            var expectedTotalRowCount = this.faker.Random.Int(1, 4);
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            await gridControl.ToggleSelectAllRowsAsync(expectedState);

            var actualRowStates = await gridControl.GetRowSelectionStatesAsync();

            Assert.That(actualRowStates, Is.All.EqualTo(expectedState).And.Exactly(expectedTotalRowCount).Items);
        }

        /// <summary>
        /// Tests that <see cref="IReadOnlyGrid.ToggleAllRowsAsync"/> does not change any row states when there are no rows in the grid.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ToggleAllRows_NoRowsInGrid_ShouldNotChangeRowStates()
        {
            var gridControl = await this.SetupReadOnlyGridScenarioAsync(withRelatedRecords: Enumerable.Empty<RelatedRecordFaker>());

            await gridControl.ToggleSelectAllRowsAsync(select: true);

            var actualRowCount = await gridControl.GetTotalRowCountAsync();
            var actualRowStates = await gridControl.GetRowSelectionStatesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualRowCount, Is.EqualTo(0));
                Assert.That(actualRowStates, Is.Empty);
            });
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