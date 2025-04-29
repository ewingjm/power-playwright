namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IUpdMSPicklist"/> PCF control class.
    /// </summary>
    public class IUpdMSPicklistTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the Multi Select Picklist control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IUpdMSPicklist.SetValueAsync(int?[])"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            int[]? values = Enum.GetValues(typeof(pp_record_pp_choices))
                   .Cast<int>()
                   .ToArray();

            var choices = new List<pp_record_pp_choices>()
            {
                 pp_record_pp_choices.ChoiceA,
                 pp_record_pp_choices.ChoiceB,
                 pp_record_pp_choices.ChoiceC,
            };

            var multiSelectPicklist = await this.SetupMultiChoicesScenarioAsync(choices);

            await multiSelectPicklist.SetValueAsync(values);

            Assert.That(multiSelectPicklist.GetValueAsync, Is.EqualTo(values));
        }

        /// <summary>
        /// Tests that <see cref="IUpdMSPicklist.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var multiSelectPicklist = await this.SetupMultiChoicesScenarioAsync();

            Assert.That(multiSelectPicklist.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a Multi Select Picklist control scenario for testing by creating a record with a specified or generated URL.
        /// </summary>
        /// <param name="withChoices">An optional URL string to set in the record. If null, Random choices be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUpdMSPicklist"/>.</returns>
        private async Task<IUpdMSPicklist> SetupMultiChoicesScenarioAsync(IEnumerable<pp_record_pp_choices>? withChoices = null)
        {
            var record = new RecordFaker();

            if (withChoices != null)
            {
                record.RuleFor(x => x.pp_choices, withChoices);
            }
            else
            {
                record.Ignore(p => p.pp_choices);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetField<IUpdMSPicklist>(nameof(pp_Record.pp_choices)).Control;
        }
    }
}