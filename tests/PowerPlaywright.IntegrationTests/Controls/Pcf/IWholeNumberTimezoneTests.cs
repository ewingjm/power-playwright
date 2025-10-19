namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IWholeNumberTimezone"/> PCF control class.
    /// </summary>
    public class IWholeNumberTimezoneTests : IntegrationTests
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
        /// Tests that <see cref="IWholeNumberTimezone.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var timezoneControl = await this.SetupTimezoneScenarioAsync(withNoValue: true);

            Assert.That(await timezoneControl.GetValueAsync(), Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumberTimezone.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = "(GMT-08:00) Pacific Time (US & Canada)";
            var timezoneControl = await this.SetupTimezoneScenarioAsync(withValue: expectedValue);

            Assert.That(await timezoneControl.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumberTimezone.GetAllOptionsAsync"/> returns the available timezone options.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAllOptionsAsync_ActiveRecord_ReturnsOptions()
        {
            var timezoneControl = await this.SetupTimezoneScenarioAsync();
            var options = await timezoneControl.GetAllOptionsAsync();

            Assert.That(options, Is.Not.Empty);
            Assert.That(options, Contains.Item("(GMT-08:00) Pacific Time (US & Canada)"));
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumberTimezone.GetAllOptionsAsync"/> returns an exception if the record is inactive or the field is disabled as all option set values can't be read.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAllOptionsAsync_DisabledField_ThrowsException()
        {
            var timezoneControl = await this.SetupTimezoneScenarioAsync(withDisabledRecord: true);

            Assert.ThrowsAsync<PowerPlaywrightException>(
                () => timezoneControl.GetAllOptionsAsync());
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumberTimezone.GetValueAsync()"/> returns the value when the record is inactive.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_InactiveRecord_ReturnsValue()
        {
            var expectedValue = "(GMT-05:00) Eastern Time (US & Canada)";
            var timezoneControl = await this.SetupTimezoneScenarioAsync(withValue: expectedValue, withDisabledRecord: true);

            Assert.That(await timezoneControl.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumberTimezone.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var timezoneControl = await this.SetupTimezoneScenarioAsync(withNoValue: true);
            var expectedValue = "(GMT+01:00) Brussels, Copenhagen, Madrid, Paris";

            await timezoneControl.SetValueAsync(expectedValue);

            Assert.That(await timezoneControl.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumberTimezone.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var timezoneControl = await this.SetupTimezoneScenarioAsync(withValue: "(GMT-08:00) Pacific Time (US & Canada)");
            var expectedValue = "(GMT-05:00) Eastern Time (US & Canada)";

            await timezoneControl.SetValueAsync(expectedValue);

            Assert.That(await timezoneControl.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a Timezone control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional timezone value to set in the record.</param>
        /// <param name="withNoValue">Whether to set the timezone to null. Defaults to false.</param>
        /// <param name="withDisabledRecord">Whether or not to make the record inactive. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IWholeNumberTimezone"/>.</returns>
        private async Task<IWholeNumberTimezone> SetupTimezoneScenarioAsync(string? withValue = null, bool withNoValue = false, bool withDisabledRecord = false)
        {
            var record = new RecordFaker();

            if (withDisabledRecord)
            {
                record.RuleFor(r => r.statecode, r => pp_record_statecode.Inactive);
                record.RuleFor(r => r.statuscode, r => pp_record_statuscode.Inactive);
            }

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_wholenumbertimezone, f => (int?)null);
            }
            else if (withValue is not null)
            {
                // Get timezone code from timezone string
                var timezoneCode = this.GetTimezoneCodeFromString(withValue);
                record.RuleFor(x => x.pp_wholenumbertimezone, timezoneCode);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IWholeNumberTimezone>(nameof(pp_Record.pp_wholenumbertimezone)).Control;
        }

        /// <summary>
        /// Maps a timezone display string to its corresponding timezone code.
        /// This is a helper method to convert human-readable timezone strings to their numeric codes.
        /// </summary>
        /// <param name="timezoneString">The timezone display string (e.g., "(GMT-08:00) Pacific Time (US & Canada)").</param>
        /// <returns>The timezone code as an integer.</returns>
        private int GetTimezoneCodeFromString(string timezoneString)
        {
            // Map common timezones to their codes
            // These codes are standard Dynamics 365/Dataverse timezone codes
            return timezoneString switch
            {
                "(GMT-08:00) Pacific Time (US & Canada)" => 4,
                "(GMT-05:00) Eastern Time (US & Canada)" => 35,
                "(GMT+01:00) Brussels, Copenhagen, Madrid, Paris" => 105,
                "(GMT) Greenwich Mean Time : Dublin, Edinburgh, Lisbon, London" => 85,
                _ => 4, // Default to Pacific Time
            };
        }
    }
}
