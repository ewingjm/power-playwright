namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IWholeNumber"/> PCF control class.
    /// </summary>
    public class IWholeNumberTests : IntegrationTests
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
        /// Tests that <see cref="IWholeNumber.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var wholeNumberControl = await this.SetupWholeNumberScenarioAsync(withNoValue: true);

            Assert.That(wholeNumberControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumber.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Random.Number();
            var wholeNumberControl = await this.SetupWholeNumberScenarioAsync(withValue: expectedValue);

            Assert.That(wholeNumberControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumber.SetValueAsync(int?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var wholeNumberControl = await this.SetupWholeNumberScenarioAsync(withNoValue: true);
            var expectedValue = this.faker.Random.Number();

            await wholeNumberControl.SetValueAsync(expectedValue);

            Assert.That(wholeNumberControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumber.SetValueAsync(int?)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var wholeNumberControl = await this.SetupWholeNumberScenarioAsync();
            var expectedValue = this.faker.Random.Number();

            await wholeNumberControl.SetValueAsync(expectedValue);

            Assert.That(wholeNumberControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IWholeNumber"/>.</returns>
        private async Task<IWholeNumber> SetupWholeNumberScenarioAsync(int? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_wholenumbernone, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_wholenumbernone, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField(pp_Record.Forms.Information.WholeNumberNone).GetControl<IWholeNumber>();
        }
    }
}