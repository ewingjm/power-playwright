namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using Microsoft.Xrm.Sdk;
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
        /// Sets up the test dependencies.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }

        /// <summary>
        /// Tests that <see cref="ICurrency.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var currencyControl = await this.SetupCurrencyScenarioAsync(withNoValue: true);

            Assert.That(currencyControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="ICurrency.SetValueAsync(decimal?)"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Finance.Amount();
            var currencyControl = await this.SetupCurrencyScenarioAsync(withCurrency: expectedValue);

            Assert.That(currencyControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="ICurrency.SetValueAsync(decimal?)"/> sets the value when the control does not contain a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var currencyControl = await this.SetupCurrencyScenarioAsync(withNoValue: true);
            var expectedValue = this.faker.Finance.Amount();

            await currencyControl.SetValueAsync(expectedValue);

            Assert.That(currencyControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="ICurrency.SetValueAsync(decimal?)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var currencyControl = await this.SetupCurrencyScenarioAsync();
            var expectedValue = this.faker.Finance.Amount();

            await currencyControl.SetValueAsync(expectedValue);

            Assert.That(currencyControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a currency control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withCurrency">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ICurrencyControl"/>.</returns>
        private async Task<ICurrency> SetupCurrencyScenarioAsync(decimal? withCurrency = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(p => p.pp_currency, f => null);
            }
            else if (withCurrency.HasValue && !withNoValue)
            {
                record.RuleFor(p => p.pp_currency, f => new Money(withCurrency.Value));
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<ICurrency>(nameof(pp_Record.pp_currency)).Control;
        }
    }
}