namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the IUrlControl PCF Control.
    /// </summary>
    internal class IUrlControlTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the url control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="ISettableControl.SetValueAsync(string)"/> returns string when the field has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var urlControl = await this.SetupUrlScenarioAsync(withRecord: this.GetNamedRecordFaker(out var recordName));

            await urlControl.SetValueAsync(recordName);

            Assert.That(urlControl.GetValueAsync, Is.EqualTo(recordName));
        }

        /// <summary>
        /// Tests that <see cref="ISettableControl.GetValueAsync"/> returns string when the field has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainValue_ReturnsValue()
        {
            var urlControl = await this.SetupUrlScenarioAsync(withRelatedRecord: null);

            Assert.That(urlControl.GetValueAsync, Is.Not.Null);
        }

        /// <summary>
        /// Tests that <see cref="ISettableControl.GetValueAsync"/> returns empty string when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsEmptyString()
        {
            var urlControl = await this.SetupUrlScenarioAsync(withRecord: null, generateNull: true);

            Assert.That(urlControl.GetValueAsync, Is.EqualTo("---"));
        }

        private async Task<IUrlControl> SetupUrlScenarioAsync(Faker<pp_Record>? withRecord = null, Faker<pp_RelatedRecord>? withRelatedRecord = null, IEnumerable<Faker<pp_RelatedRecord>>? withRelatableRecords = null, bool generateNull = false)
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

            if (generateNull)
            {
                withRecord.RuleFor(x => x.pp_singlelineoftexturl, f => string.Empty);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(withRecord.Generate());

            return recordPage.Form.GetControl<IUrlControl>(nameof(pp_Record.pp_singlelineoftexturl));
        }

        private Faker<pp_Record> GetNamedRecordFaker(out string recordName)
        {
            var url = this.faker.Internet.Url();
            recordName = url;

            return new RecordFaker().RuleFor(r => r.pp_singlelineoftexturl, f => url);
        }
    }
}