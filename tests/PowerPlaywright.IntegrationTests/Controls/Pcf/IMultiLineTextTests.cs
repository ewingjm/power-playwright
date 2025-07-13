namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IMultiLineTextTests"/> PCF control class.
    /// </summary>
    public class IMultiLineTextTests : IntegrationTests
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
        /// Tests that <see cref="IMultiLineText.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var multiLineTextAreaControl = await this.SetupMultiLineTextScenarioAsync(withNoValue: true);

            Assert.That(multiLineTextAreaControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IMultiLineText.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.GenerateRandomText();
            var multiLineTextAreaControl = await this.SetupMultiLineTextScenarioAsync(withValue: expectedValue);

            Assert.That(multiLineTextAreaControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IMultiLineText.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var multiLineTextAreaControl = await this.SetupMultiLineTextScenarioAsync(withNoValue: true);
            var expectedValue = this.GenerateRandomText();

            await multiLineTextAreaControl.SetValueAsync(expectedValue);

            Assert.That(multiLineTextAreaControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IMultiLineText.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var multiLineTextAreaControl = await this.SetupMultiLineTextScenarioAsync();
            var expectedValue = this.GenerateRandomText();

            await multiLineTextAreaControl.SetValueAsync(expectedValue);

            Assert.That(multiLineTextAreaControl.GetValueAsync, Is.EqualTo(expectedValue));
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IMultiLineText"/>.</returns>
        private async Task<IMultiLineText> SetupMultiLineTextScenarioAsync(string? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_multiplelinesoftexttext, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_multiplelinesoftexttext, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IMultiLineText>(nameof(pp_Record.pp_multiplelinesoftexttext)).Control;
        }
    }
}