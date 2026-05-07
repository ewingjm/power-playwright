namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IPowerAppsOneGrid"/> control class.
    /// </summary>
    public class IPowerAppsOneGridTests : IntegrationTests
    {
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
        /// Tests that <see cref="IPowerAppsOneGrid.ExpandNestedSubgridAsync"/> always returns a nested subgrid.
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

        private async Task<IPowerAppsOneGrid> SetupNestedEditableGridScenarioAsync(Faker<pp_Record>? withRecord = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatedRecords = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatableRecords = null)
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

            return recordPage.Form.GetDataSet(pp_Record.Forms.Information.RelatedPowerAppsOneNestedSubGrid).GetControl<IPowerAppsOneGrid>();
        }
    }
}