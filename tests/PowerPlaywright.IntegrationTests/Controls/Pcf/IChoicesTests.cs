namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.IntegrationTests.Extensions;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Extensions;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IChoices"/> PCF control class.
    /// </summary>
    public class IChoicesTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the test dependencies.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }

        /// <summary>
        /// Tests that <see cref="IChoices.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var choicesControl = await this.SetupChoicesScenarioAsync(withNoValue: true);

            Assert.That(choicesControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IChoices.GetValueAsync()"/> returns the value when a value is set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Random.EnumValuesRange<pp_record_pp_choices>(1);
            var choicesControl = await this.SetupChoicesScenarioAsync(expectedValue);

            Assert.That(choicesControl.GetValueAsync, Is.EquivalentTo(expectedValue.Select(v => v.ToDisplayName()).ToArray()));
        }

        /// <summary>
        /// Tests that <see cref="IChoice.SetValueAsync(string)"/> sets the value when the control does not contain a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var choicesControl = await this.SetupChoicesScenarioAsync(withNoValue: true);
            var expectedValue = this.faker.Random.EnumValuesRange<pp_record_pp_choices>(1).Select(v => v.ToDisplayName()).ToArray();

            await choicesControl.SetValueAsync(expectedValue);

            Assert.That(choicesControl.GetValueAsync, Is.EquivalentTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IChoices.SetValueAsync(IEnumerable{string})"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var choicesControl = await this.SetupChoicesScenarioAsync();
            var expectedValue = this.faker.Random.EnumValuesRange<pp_record_pp_choices>(1).Select(v => v.ToDisplayName()).ToArray();

            await choicesControl.SetValueAsync(expectedValue);

            Assert.That(choicesControl.GetValueAsync, Is.EquivalentTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IChoices.SelectAllAsync()"/> sets the value to all available choices.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SelectAllAsync_Always_SetsValueToAllValues()
        {
            var choicesControl = await this.SetupChoicesScenarioAsync();
            var allValues = Enum.GetValues(typeof(pp_record_pp_choices)).Cast<pp_record_pp_choices>().Select(v => v.ToDisplayName()).ToArray();

            await choicesControl.SelectAllAsync();

            Assert.That(choicesControl.GetValueAsync, Is.EquivalentTo(allValues));
        }

        /// <summary>
        /// Sets up a choice control scenario for testing by creating a record with a specified or generated choices.
        /// </summary>
        /// <param name="withValue">An optional choice value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IChoices"/>.</returns>
        private async Task<IChoices> SetupChoicesScenarioAsync(IEnumerable<pp_record_pp_choices>? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_choices, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_choices, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetField(nameof(pp_Record.pp_choices)).GetControl<IChoices>();
        }
    }
}