namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IDecimalNumber"/> PCF control class.
    /// </summary>
    public class IDecimalTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the Decimal control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IDecimalNumber.SetValueAsync(decimal?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var decimalValue = this.faker.Finance.Amount();
            var decimalControl = await this.SetupDecimalScenarioAsync();

            await decimalControl.SetValueAsync(decimalValue);

            Assert.That(decimalControl.GetValueAsync, Is.EqualTo(decimalValue));
        }

        /// <summary>
        /// Tests that <see cref="IDecimalNumber.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var decimalControl = await this.SetupDecimalScenarioAsync(withDecimal: false);

            Assert.That(decimalControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a Decimal control scenario for testing by creating a record with a specified or generated decimal.
        /// </summary>
        /// <param name="withDecimal">An optional decimal Value to set in the record. If null, a random decimal will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IDecimalNumber"/>.</returns>
        private async Task<IDecimalNumber> SetupDecimalScenarioAsync(bool withDecimal = true)
        {
            var record = new RecordFaker();

            if (!withDecimal)
            {
                record.Ignore(p => p.pp_decimal);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IDecimalNumber>(nameof(pp_Record.pp_decimal)).Control;
        }
    }
}