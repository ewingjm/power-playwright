namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Globalization;
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
        /// <summary>
        /// Tests that <see cref="IDateTime.SetValueAsync(DateTime?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var dateValue = DateTime.UtcNow;
            var dateControl = await this.SetupDateTimeScenarioAsync(dateValue);

            await dateControl.SetValueAsync(dateValue);

            var expected = $"{dateValue.ToShortDateString()} {dateValue.ToShortTimeString()}";

            Assert.That(await dateControl.GetValueAsync(), Is.EqualTo(expected));
        }

        /// <summary>
        /// Tests that <see cref="IDateTime.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var dateControl = await this.SetupDateTimeScenarioAsync();

            Assert.That(dateControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a URL control scenario for testing by creating a record with a specified or generated DateTime.
        /// </summary>
        /// <param name="withDate">An optional DateTime Value to set in the record. If null, a random DateTime will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IDateTimeControl"/>.</returns>
        private async Task<IDateTime> SetupDateTimeScenarioAsync(DateTime? withDate = null)
        {
            var record = new RecordFaker();

            if (!withDate.HasValue)
            {
                record.Ignore(p => p.pp_dateandtimedateandtime);
            }
            else
            {
                record.RuleFor(x => x.pp_dateandtimedateandtime, withDate);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetControl<IDateTime>(nameof(pp_Record.pp_dateandtimedateandtime));
        }
    }
}