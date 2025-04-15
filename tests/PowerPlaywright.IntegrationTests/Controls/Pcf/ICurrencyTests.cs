namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="ICurrency"/> PCF control class.
    /// </summary>
    public class ICurrencyTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the currency control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="ICurrency.SetValueAsync(decimal?)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var currency = this.faker.Finance.Amount();
            var currencyControl = await this.SetupCurrencyScenarioAsync();

            await currencyControl.SetValueAsync(currency);

            Assert.That(currencyControl.GetValueAsync, Is.EqualTo(currency));
        }

        /// <summary>
        /// Tests that <see cref="ICurrency.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var currencyControl = await this.SetupCurrencyScenarioAsync(withCurrency: false);

            Assert.That(currencyControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a currency control scenario for testing by creating a record with a specified or generated currency.
        /// </summary>
        /// <param name="withCurrency">An optional Currency Value to set in the record. If null, a random currency will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ICurrencyControl"/>.</returns>
        private async Task<ICurrency> SetupCurrencyScenarioAsync(bool withCurrency = true)
        {
            var record = new RecordFaker();

            if (!withCurrency)
            {
                record.Ignore(p => p.pp_currency);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetControl<ICurrency>(nameof(pp_Record.pp_currency));
        }
    }
}