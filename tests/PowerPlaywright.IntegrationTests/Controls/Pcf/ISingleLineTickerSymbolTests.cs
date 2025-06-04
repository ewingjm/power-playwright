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
        /// Sets up the test dependencies.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineTickerSymbol.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var singleLineTickerSymbolControl = await this.SetupSingleLineTickerSymbolScenarioAsync(withNoValue: true);

            Assert.That(singleLineTickerSymbolControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineTickerSymbol.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Lorem.Letter(3).ToUpper();
            var singleLineTickerSymbolControl = await this.SetupSingleLineTickerSymbolScenarioAsync(withValue: expectedValue);

            Assert.That(singleLineTickerSymbolControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineTickerSymbol.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var singleLineTickerSymbolControl = await this.SetupSingleLineTickerSymbolScenarioAsync(withNoValue: true);
            var expectedValue = this.faker.Lorem.Letter(3).ToUpper();

            await singleLineTickerSymbolControl.SetValueAsync(expectedValue);

            Assert.That(singleLineTickerSymbolControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineTickerSymbol.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var singleLineTickerSymbolControl = await this.SetupSingleLineTickerSymbolScenarioAsync();
            var expectedValue = this.faker.Lorem.Letter(3).ToUpper();

            await singleLineTickerSymbolControl.SetValueAsync(expectedValue);

            Assert.That(singleLineTickerSymbolControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ISingleLineTickerSymbol"/>.</returns>
        private async Task<ISingleLineTickerSymbol> SetupSingleLineTickerSymbolScenarioAsync(string? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_singlelineoftexttickersymbol, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_singlelineoftexttickersymbol, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<ISingleLineTickerSymbol>(nameof(pp_Record.pp_singlelineoftexttickersymbol)).Control;
        }
    }
}