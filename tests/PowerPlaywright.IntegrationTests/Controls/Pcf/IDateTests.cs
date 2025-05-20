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
        /// Sets up the Date control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IDate.SetValueAsync(DateTime?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var dateValue = this.faker.Date.Recent().Date;
            var dateControl = await this.SetupDateScenarioAsync();

            await dateControl.SetValueAsync(dateValue);

            Assert.That(dateControl.GetValueAsync, Is.EqualTo(dateValue));
        }

        /// <summary>
        /// Tests that <see cref="IDate.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var dateControl = await this.SetupDateScenarioAsync(withDate: false);

            Assert.That(dateControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a Date control scenario for testing by creating a record with a specified or generated Date.
        /// </summary>
        /// <param name="withDate">An optional Date Value to set in the record. If null, a random Date will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IDateControl"/>.</returns>
        private async Task<IDate> SetupDateScenarioAsync(bool withDate = true)
        {
            var record = new RecordFaker();

            if (!withDate)
            {
                record.Ignore(p => p.pp_dateandtimedateonly);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IDate>(nameof(pp_Record.pp_dateandtimedateonly)).Control;
        }
    }
}