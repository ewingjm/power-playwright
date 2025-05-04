namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="ISingleLineUrl"/> PCF control class.
    /// </summary>
    public class IOptionSetTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the optionset control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IOptionSet.SetValueAsync(string)"/> sets the value with either the default or the passed in value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <param name="choiceText">Choice Text to assert against.</param>
        /// <param name="choice">The enumerated choice to use as part of the setup.</param>
        [TestCase("Choice A", null)]
        [TestCase("Choice C", pp_record_pp_choice.ChoiceC)]
        public async Task GetValueAsync_ReturnsValue(string choiceText, pp_record_pp_choice? choice)
        {
            var optionSetControl = await this.SetupOptionSetScenarioAsync(choice);

            if (!choice.HasValue)
            {
                await optionSetControl.SetValueAsync(choiceText);
            }

            Assert.That(optionSetControl.GetValueAsync, Is.EqualTo(choiceText));
        }

        /// <summary>
        /// Tests that <see cref="IOptionSet.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var optionSetControl = await this.SetupOptionSetScenarioAsync();

            Assert.That(optionSetControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a OptionSet (Single value) control scenario for testing by creating a record with a specified or generated choices.
        /// </summary>
        /// <param name="withOptionSet">An optional choice value to set in the record. If null, it will leave the choice null.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IOptionSet"/>.</returns>
        private async Task<IOptionSet> SetupOptionSetScenarioAsync(pp_record_pp_choice? withOptionSet = null)
        {
            var record = new RecordFaker();

            if (withOptionSet is not null)
            {
                record.RuleFor(x => x.pp_choice, withOptionSet);
            }
            else
            {
                record.Ignore(x => x.pp_choice);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetField<IOptionSet>(nameof(pp_Record.pp_choice)).Control;
        }
    }
}