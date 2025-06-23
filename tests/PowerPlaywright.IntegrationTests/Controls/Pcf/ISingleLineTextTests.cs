namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="ISingleLineText"/> PCF control class.
    /// </summary>
    public class ISingleLineTextTests : IntegrationTests
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
        /// Tests that <see cref="ISingleLineText.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var singleLineTextControl = await this.SetupSingleLineTextScenarioAsync(withNoValue: true);

            Assert.That(singleLineTextControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineText.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.GenerateRandomText();
            var singleLineTextControl = await this.SetupSingleLineTextScenarioAsync(withValue: expectedValue);

            Assert.That(singleLineTextControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineText.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var singleLineTextControl = await this.SetupSingleLineTextScenarioAsync(withNoValue: true);
            var expectedValue = this.GenerateRandomText();

            await singleLineTextControl.SetValueAsync(expectedValue);

            Assert.That(singleLineTextControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineText.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var singleLineTextControl = await this.SetupSingleLineTextScenarioAsync();
            var expectedValue = this.GenerateRandomText();

            await singleLineTextControl.SetValueAsync(expectedValue);

            Assert.That(singleLineTextControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        private string GenerateRandomText()
        {
            return string.Join(" ", this.faker.Lorem.Words(5));
        }

        /// <summary>
        /// Sets up a control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ISingleLineText"/>.</returns>
        private async Task<ISingleLineText> SetupSingleLineTextScenarioAsync(string? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_singlelineoftexttext, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_singlelineoftexttext, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField(nameof(pp_Record.pp_singlelineoftexttext)).GetControl<ISingleLineText>();
        }
    }
}