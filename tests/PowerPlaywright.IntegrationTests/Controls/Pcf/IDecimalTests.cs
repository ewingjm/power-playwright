namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IDecimal"/> PCF control class.
    /// </summary>
    public class IDecimalTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the url control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IDecimal.SetValueAsync(decimal)"/> sets the value.
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
        /// Tests that <see cref="ISingleLineUrl.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var currencyControl = await this.SetupDecimalScenarioAsync(withDecimal: false);

            Assert.That(currencyControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a URL control scenario for testing by creating a record with a specified or generated Currency.
        /// </summary>
        /// <param name="withDecimal">An optional Currency Value to set in the record. If null, a random Currency will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ICurrencyControl"/>.</returns>
        private async Task<ICurrency> SetupDecimalScenarioAsync(bool withDecimal = true)
        {
            var record = new RecordFaker();

            if (!withDecimal)
            {
                record.Ignore(p => p.pp_decimal);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetControl<ICurrency>(nameof(pp_Record.pp_decimal));
        }
    }
}