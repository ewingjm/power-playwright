namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="ISingleLineFloatingPointNumber"/> PCF control class.
    /// </summary>
    public class ISingleLineFloatingPointNumberTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the floating point number control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineFloatingPointNumber.SetValueAsync(double)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var floatAmount = this.faker.Random.Double(1.0, 100.0);
            var floatControl = await this.SetupFloatScenarioAsync();

            await floatControl.SetValueAsync(floatAmount);

            Assert.That(floatControl.GetValueAsync, Is.EqualTo(floatAmount));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineUrl.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var currencyControl = await this.SetupFloatScenarioAsync(withFloat: false);

            Assert.That(currencyControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a URL control scenario for testing by creating a record with a specified or generated Float.
        /// </summary>
        /// <param name="withFloat">An optional Floating Number Value to set in the record. If null, a random Amount will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IFloatingPointNumberControl"/>.</returns>
        private async Task<ISingleLineFloatingPointNumber> SetupFloatScenarioAsync(bool withFloat = true)
        {
            var record = new RecordFaker();

            if (!withFloat)
            {
                record.Ignore(p => p.pp_float);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetControl<ISingleLineFloatingPointNumber>(nameof(pp_Record.pp_float));
        }
    }
}