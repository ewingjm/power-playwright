namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IDateTime"/> PCF control class.
    /// </summary>
    public class IDateTimeTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the Datetime control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IDateTime.SetValueAsync(DateTime?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Ignore("Experimental")]
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var dateValue = this.faker.Date.Recent();
            var dateControl = await this.SetupDateTimeScenarioAsync();

            await dateControl.SetValueAsync(dateValue);

            var actualDate = await dateControl.GetValueAsync();

            var expected = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, dateValue.Hour, dateValue.Minute, 0, dateValue.Kind);
            var actual = new DateTime(actualDate.Value.Year, actualDate.Value.Month, actualDate.Value.Day, actualDate.Value.Hour, actualDate.Value.Minute, 0, actualDate.Value.Kind);

            Assert.That(actual, Is.EqualTo(expected), "Set and retrieved DateTime do not match");
        }

        /// <summary>
        /// Tests that <see cref="IDateTime.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var dateControl = await this.SetupDateTimeScenarioAsync(withDate: false);

            Assert.That(dateControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a URL control scenario for testing by creating a record with a specified or generated DateTime.
        /// </summary>
        /// <param name="withDate">An optional DateTime Value to set in the record. If null, a random DateTime will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IDateTimeControl"/>.</returns>
        private async Task<IDateTime> SetupDateTimeScenarioAsync(bool withDate = true)
        {
            var record = new RecordFaker();

            if (!withDate)
            {
                record.Ignore(p => p.pp_dateandtimedateandtime);
            }
            else
            {
                record.RuleFor(x => x.pp_dateandtimedateandtime, DateTime.Now);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetControl<IDateTime>(nameof(pp_Record.pp_dateandtimedateandtime));
        }
    }
}