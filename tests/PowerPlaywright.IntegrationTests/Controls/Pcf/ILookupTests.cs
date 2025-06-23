namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IReadOnlyGrid"/> control class.
    /// </summary>
    public class ILookupTests : IntegrationTests
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
        /// Tests that <see cref="ILookup.GetValueAsync"/> returns null when the lookup has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var lookupControl = await this.SetupLookupScenarioAsync(withRelatedRecord: null);

            Assert.That(lookupControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="ILookup.GetValueAsync"/> returns the value of the lookup when set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var lookupControl = await this.SetupLookupScenarioAsync(withRelatedRecord: this.GetNamedRelatedRecordFaker(out var relatedRecordName));

            Assert.That(lookupControl.GetValueAsync, Is.EqualTo(relatedRecordName));
        }

        /// <summary>
        /// Tests that <see cref="ILookup.SetValueAsync(string)"/> sets the lookup to the first search result returned when the value is entered.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_SearchReturnsResults_SetsValueToFirstResult()
        {
            var lookupControl = await this.SetupLookupScenarioAsync(withRelatableRecords: [this.GetNamedRelatedRecordFaker(out var relatableRecordName)]);

            await lookupControl.SetValueAsync(relatableRecordName);

            Assert.That(lookupControl.GetValueAsync, Is.EqualTo(relatableRecordName));
        }

        /// <summary>
        /// Tests that <see cref="ILookup.SetValueAsync(string)"/> throws a <see cref="NotFoundException"/> if the value returns no search results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_SearchReturnsNoResults_ThrowsNotFoundException()
        {
            var lookupControl = await this.SetupLookupScenarioAsync(withRelatableRecords: null);

            Assert.ThrowsAsync<NotFoundException>(
                () => lookupControl.SetValueAsync(this.faker.Random.AlphaNumeric(100)));
        }

        /// <summary>
        /// Tests that <see cref="ILookup.SetValueAsync(string)"/> replaces an existing value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var lookupControl = await this.SetupLookupScenarioAsync(
                withRelatedRecord: this.GetNamedRelatedRecordFaker(out var relatedRecordName),
                withRelatableRecords: [this.GetNamedRelatedRecordFaker(out var relatableRecordName)]);

            await lookupControl.SetValueAsync(relatableRecordName);

            Assert.That(lookupControl.GetValueAsync, Is.EqualTo(relatableRecordName));
        }

        private async Task<ILookup> SetupLookupScenarioAsync(Faker<pp_Record>? withRecord = null, Faker<pp_RelatedRecord>? withRelatedRecord = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatableRecords = null)
        {
            withRecord ??= new RecordFaker();

            if (withRelatedRecord != null)
            {
                withRecord.RuleFor(r => r.pp_relatedrecord_record, f => withRelatedRecord?.Generate());
            }

            if (withRelatableRecords != null)
            {
                withRecord.RuleFor(r => r.pp_Record_RelatedRecord, f => withRelatableRecords?.Select(f => f.Generate()));
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(withRecord.Generate());

            return recordPage.Form.GetField(pp_Record.Forms.Information.RelatedRecord).GetControl<ILookup>();
        }

        private Faker<pp_RelatedRecord> GetNamedRelatedRecordFaker(out string relatedRecordName)
        {
            var name = this.faker.Lorem.Random.AlphaNumeric(100);
            relatedRecordName = name;

            return new RelatedRecordFaker().RuleFor(r => r.pp_Name, f => name);
        }
    }
}