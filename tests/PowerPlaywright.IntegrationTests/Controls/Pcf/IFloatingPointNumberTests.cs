namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;
    using System.Threading.Tasks;

    /// <summary>
    /// Tests the <see cref="IFloatingPointNumber"/> PCF control class.
    /// </summary>
    public class IFloatingPointNumberTests : IntegrationTests
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
        /// Tests that <see cref="IFloatingPointNumber.SetValueAsync(double?)"/> sets the value.
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
        /// Tests that <see cref="IFloatingPointNumber.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var floatControl = await this.SetupFloatScenarioAsync(withFloat: false);

            Assert.That(floatControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a Floating Number control scenario for testing by creating a record with a specified or generated Float.
        /// </summary>
        /// <param name="withFloat">An optional Floating Number Value to set in the record. If null, a random Amount will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IFloatingPointNumberControl"/>.</returns>
        private async Task<IFloatingPointNumber> SetupFloatScenarioAsync(bool withFloat = true)
        {
            var record = new RecordFaker();

            if (!withFloat)
            {
                record.Ignore(p => p.pp_float);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetControl<IFloatingPointNumber>(nameof(pp_Record.pp_float));
        }
    }
}