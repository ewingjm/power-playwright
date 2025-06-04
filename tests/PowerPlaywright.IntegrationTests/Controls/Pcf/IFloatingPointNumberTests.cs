namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IFloatingPointNumber"/> PCF control class.
    /// </summary>
    public class IFloatingPointNumberTests : IntegrationTests
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
        /// Tests that <see cref="IFloatingPointNumber.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var floatingPointControl = await this.SetupFloatingPointScenarioAsync(withNoValue: true);

            Assert.That(floatingPointControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IFloatingPointNumber.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.GetRandomFloatingPoint();
            var floatingPointControl = await this.SetupFloatingPointScenarioAsync(withValue: expectedValue);

            Assert.That(floatingPointControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IFloatingPointNumber.SetValueAsync(double?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var floatingPointControl = await this.SetupFloatingPointScenarioAsync(withNoValue: true);
            var expectedValue = this.GetRandomFloatingPoint();

            await floatingPointControl.SetValueAsync(expectedValue);

            Assert.That(floatingPointControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IFloatingPointNumber.SetValueAsync(double?)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var floatingPointControl = await this.SetupFloatingPointScenarioAsync();
            var expectedValue = this.GetRandomFloatingPoint();

            await floatingPointControl.SetValueAsync(expectedValue);

            Assert.That(floatingPointControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        private double GetRandomFloatingPoint()
        {
            return Math.Round(this.faker.Random.Double(), 2);
        }

        /// <summary>
        /// Sets up a floating-point control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IFloatingPointNumber"/>.</returns>
        private async Task<IFloatingPointNumber> SetupFloatingPointScenarioAsync(double? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_float, f => null);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_float, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IFloatingPointNumber>(nameof(pp_Record.pp_float)).Control;
        }
    }
}