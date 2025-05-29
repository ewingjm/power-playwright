namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IDecimalNumber"/> PCF control class.
    /// </summary>
    public class IDecimalNumberTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the test dependencies.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IDecimalNumber.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var decimalControl = await this.SetupDecimalScenarioAsync(withNoValue: true);

            Assert.That(decimalControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IDecimalNumber.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Random.Decimal();
            var decimalControl = await this.SetupDecimalScenarioAsync(withValue: expectedValue);

            Assert.That(decimalControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IDecimalNumber.SetValueAsync(decimal?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var decimalControl = await this.SetupDecimalScenarioAsync(withNoValue: true);
            var expectedValue = this.faker.Random.Decimal();

            await decimalControl.SetValueAsync(expectedValue);

            Assert.That(decimalControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IDecimalNumber.SetValueAsync(decimal?)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var decimalControl = await this.SetupDecimalScenarioAsync();
            var expectedValue = this.faker.Random.Decimal();

            await decimalControl.SetValueAsync(expectedValue);

            Assert.That(decimalControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a Decimal control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IDecimalNumber"/>.</returns>
        private async Task<IDecimalNumber> SetupDecimalScenarioAsync(decimal? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_decimal, f => null);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_decimal, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IDecimalNumber>(nameof(pp_Record.pp_decimal)).Control;
        }
    }
}