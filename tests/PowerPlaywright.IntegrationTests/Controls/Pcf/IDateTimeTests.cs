namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.IntegrationTests.Extensions;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IDateTime"/> PCF control class.
    /// </summary>
    public class IDateTimeTests : IntegrationTests
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
        /// Tests that <see cref="IDateTime.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var dateTimeControl = await this.SetupDateTimeScenarioAsync(withNoValue: true);

            Assert.That(dateTimeControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IDateTime.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.GetRandomDateTime();
            var dateTimeControl = await this.SetupDateTimeScenarioAsync(withValue: expectedValue);

            Assert.That(dateTimeControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IDateTime.GetValueAsync()"/> returns the value when the record is inactive.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_InactiveRecord_ReturnsValue()
        {
            var expectedValue = this.GetRandomDateTime();
            var dateTimeControl = await this.SetupDateTimeScenarioAsync(withValue: expectedValue, withDisabledRecord: true);

            Assert.That(dateTimeControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IDateTime.SetValueAsync(DateTime?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var dateTimeControl = await this.SetupDateTimeScenarioAsync(withNoValue: true);
            var expectedValue = this.GetRandomDateTime();

            await dateTimeControl.SetValueAsync(expectedValue);

            Assert.That(dateTimeControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IDateTime.SetValueAsync(DateTime?)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var dateTimeControl = await this.SetupDateTimeScenarioAsync();
            var expectedValue = this.GetRandomDateTime();

            await dateTimeControl.SetValueAsync(expectedValue);

            Assert.That(dateTimeControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        private DateTime GetRandomDateTime()
        {
            return this.faker.Date.Recent().ToUniversalTime().WithoutSeconds();
        }

        /// <summary>
        /// Sets up a DateTime control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <param name="withDisabledRecord">Whether or not to make the record inactive. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IDateControl"/>.</returns>
        private async Task<IDateTime> SetupDateTimeScenarioAsync(DateTime? withValue = null, bool withNoValue = false, bool withDisabledRecord = false)
        {
            var record = new RecordFaker();

            if (withDisabledRecord)
            {
                record.RuleFor(r => r.statecode, r => pp_record_statecode.Inactive);
                record.RuleFor(r => r.statuscode, r => pp_record_statuscode.Inactive);
            }

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_dateandtimedateandtime, f => null);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_dateandtimedateandtime, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IDateTime>(nameof(pp_Record.pp_dateandtimedateandtime)).Control;
        }
    }
}