namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Linq;
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
        /// Tests that <see cref="IGridControl.GetEditableColumnsAsync"/> always returns the editable columns.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [Ignore("Grid interactions fail due to viewport rendering - Issue #129")]
        public async Task GetEditableColumnsAsync_Always_ReturnsAllColumnNamesInOrder()
        {
            var expectedTotalRowCount = 1;
            var gridControl = await this.SetupGridControlScenarioAsync(withRelatedRecords: Enumerable.Range(0, expectedTotalRowCount).Select(i => new RelatedRecordFaker()));

            Assert.That(await gridControl.GetEditableColumnsAsync(0), Is.EqualTo(EditableColumns));
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

            Assert.Multiple(() =>
            {
                Assert.That(dataRows.ToList(), Has.Count.EqualTo(1));
                Assert.That(dataRows.Select(rd => rd["Name"]).First(), Is.EqualTo(name));
            });
        }

        /// <summary>
        /// Tests that <see cref="IGridControl.ExpandNestedSubgridAsync"/> always returns a nested subgrid.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ExpandNestedSubgridAsync_Always_ReturnsNestedSubGrid()
        {
            var expectedColumns = new[] { "Connected To", "Role (To)", "Description" };
            var gridControl = await this.SetupNestedEditableGridScenarioAsync(withRelatedRecords: Enumerable.Range(0, 2).Select(i => new RelatedRecordFaker()));

            var nestedSubgrid = await gridControl.ExpandNestedSubgridAsync(0);

            Assert.That(nestedSubgrid.GetColumnNamesAsync, Is.EqualTo(expectedColumns));
        }

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

        private async Task<IGridControl> SetupNestedEditableGridScenarioAsync(Faker<pp_Record>? withRecord = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatedRecords = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatableRecords = null)
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

            return recordPage.Form.GetDataSet(pp_Record.Forms.Information.RelatedNestedEditableSubGrid).GetControl<IGridControl>();
        }
    }
}