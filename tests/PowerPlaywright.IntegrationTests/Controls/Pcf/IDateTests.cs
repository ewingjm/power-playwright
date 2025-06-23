namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IDate"/> PCF control class.
    /// </summary>
    public class IDateTests : IntegrationTests
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
        /// Tests that <see cref="IDate.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var dateControl = await this.SetupDateScenarioAsync(withNoValue: true);

            Assert.That(dateControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IDate.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.GetRandomDate();
            var dateControl = await this.SetupDateScenarioAsync(withValue: expectedValue);

            Assert.That(dateControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IDate.SetValueAsync(DateTime?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var dateControl = await this.SetupDateScenarioAsync(withNoValue: true);
            var expectedValue = this.GetRandomDate();

            await dateControl.SetValueAsync(expectedValue);

            Assert.That(dateControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IDate.SetValueAsync(DateTime?)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var dateControl = await this.SetupDateScenarioAsync();
            var expectedValue = this.GetRandomDate();

            await dateControl.SetValueAsync(expectedValue);

            Assert.That(dateControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        private DateTime GetRandomDate()
        {
            return this.faker.Date.Recent().ToUniversalTime().Date;
        }

        /// <summary>
        /// Sets up a Date control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IDateControl"/>.</returns>
        private async Task<IDate> SetupDateScenarioAsync(DateTime? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_dateandtimedateonly, f => null);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_dateandtimedateonly, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField(nameof(pp_Record.pp_dateandtimedateonly)).GetControl<IDate>();
        }
    }
}