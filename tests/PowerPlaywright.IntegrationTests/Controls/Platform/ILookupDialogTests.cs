namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="ILookupDialog"/> control.
    /// </summary>
    public class ILookupDialogTests : IntegrationTests
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
        /// Tests that the <see cref="ILookupDialog.IsVisibleAsync"/> method returns true when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_DialogIsVisible_ReturnsTrue()
        {
            var lookupDialog = await this.SetupLookupDialogScenarioAsync();

            Assert.That(await lookupDialog.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="ILookupDialog.CancelAsync"/> method closes the dialog when visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CloseAsync_DialogIsVisible_DialogIsClosed()
        {
            var lookupDialog = await this.SetupLookupDialogScenarioAsync();

            await lookupDialog.CancelAsync();

            Assert.That(await lookupDialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that the <see cref="ILookupDialog.Lookup"/> property provides interaction with the lookup.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Lookup_DialogIsVisible_ProvidesLookupInteraction()
        {
            var expectedRecordName = this.faker.Lorem.Sentence(3);
            var relatableRecord = new RelatedRecordFaker()
                .FinishWith((f, r) => r.pp_Name = expectedRecordName);
            var lookupDialog = await this.SetupLookupDialogScenarioAsync(withRelatableRecord: relatableRecord);

            await lookupDialog.Lookup.SetValueAsync(expectedRecordName);

            Assert.That(await lookupDialog.Lookup.GetValueAsync(), Is.EqualTo(expectedRecordName));
        }

        /// <summary>
        /// Tests that the <see cref="ILookupDialog.Lookup"/> property provides interaction with the lookup.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SaveAsync_LookupValueSet_SavesSuccessfully()
        {
            var expectedRecordName = this.faker.Lorem.Sentence(3);
            var relatableRecord = new RelatedRecordFaker()
                .FinishWith((f, r) => r.pp_Name = expectedRecordName);
            var lookupDialog = await this.SetupLookupDialogScenarioAsync(withRelatableRecord: relatableRecord);

            await lookupDialog.Lookup.SetValueAsync(expectedRecordName);
            await lookupDialog.SaveAsync();

            Assert.That(await lookupDialog.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Sets up a lookup dialog scenario.
        /// </summary>
        /// <param name="withRelatableRecord">Optional relatable records.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<ILookupDialog> SetupLookupDialogScenarioAsync(Faker<pp_RelatedRecord>? withRelatableRecord = null)
        {
            if (withRelatableRecord != null)
            {
                using var client = this.GetServiceClient();

                await client.CreateAsync(withRelatableRecord.Generate());
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(new RecordFaker().Generate());

            var commandBar = recordPage.Form.GetDataSet(pp_Record.Forms.Information.RelatedRecordsSubgrid).CommandBar;

            return await commandBar.ClickCommandWithDialogAsync<ILookupDialog>("Add Existing Related Record");
        }
    }
}
