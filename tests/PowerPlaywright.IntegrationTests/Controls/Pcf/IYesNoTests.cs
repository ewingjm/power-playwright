namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IYesNo"/> control class.
    /// </summary>
    public class IYesNoTests : IntegrationTests
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
        /// Tests that <see cref="IYesNo.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Random.Bool();
            var yesNoControl = await this.SetupYesNoScenarioAsync(withValue: expectedValue);

            Assert.That(yesNoControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IYesNo.SetValueAsync(bool)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var yesNoControl = await this.SetupYesNoScenarioAsync(withNoValue: true);
            var expectedValue = this.faker.Random.Bool();

            await yesNoControl.SetValueAsync(expectedValue);

            Assert.That(yesNoControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IYesNo.SetValueAsync(bool)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var yesNoControl = await this.SetupYesNoScenarioAsync();
            var expectedValue = this.faker.Random.Bool();

            await yesNoControl.SetValueAsync(expectedValue);

            Assert.That(yesNoControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IYesNo"/>.</returns>
        private async Task<IYesNo> SetupYesNoScenarioAsync(bool? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_yesno, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_yesno, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IYesNo>(nameof(pp_Record.pp_yesno)).Control;
        }
    }
}
