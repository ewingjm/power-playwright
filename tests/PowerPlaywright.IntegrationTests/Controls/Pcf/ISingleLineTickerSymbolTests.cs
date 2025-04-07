namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="ISingleLineTickerSymbol"/> PCF control class.
    /// </summary>
    public class ISingleLineTickerSymbolTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the ticker symbol control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineTickerSymbol.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var tickerSymbol = this.faker.Lorem.Word();
            var tickerSymbolControl = await this.SetupTickerSymbolScenarioAsync();

            await tickerSymbolControl.SetValueAsync(tickerSymbol);

            Assert.That(tickerSymbolControl.GetValueAsync, Is.EqualTo(tickerSymbol));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineTickerSymbol.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var tickerSymbolControl = await this.SetupTickerSymbolScenarioAsync(withSymbol: string.Empty);

            Assert.That(tickerSymbolControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a Ticker Symbol control scenario for testing by creating a record with a specified or generated Ticker Symbol.
        /// </summary>
        /// <param name="withSymbol">An optional Ticker Symbol string to set in the record. If null, a random text value will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ISingleLineTickerSymbol"/>.</returns>
        private async Task<ISingleLineTickerSymbol> SetupTickerSymbolScenarioAsync(string? withSymbol = null)
        {
            var record = new RecordFaker();
            record.RuleFor(x => x.pp_singlelineoftexttickersymbol, f => withSymbol ?? f.Lorem.Word());

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetControl<ISingleLineTickerSymbol>(nameof(pp_Record.pp_singlelineoftexttickersymbol));
        }
    }
}