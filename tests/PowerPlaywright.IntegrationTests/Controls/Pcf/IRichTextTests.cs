namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Strategies.Controls.Pcf;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IRichTextTests"/> PCF control class.
    /// </summary>
    [Ignore("The IRichText control is currently not available on the current solution. Please Enable these when the solution has a richtext on the form.")]
    public class IRichTextTests : IntegrationTests
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
        /// Tests that <see cref="IRichText.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var richTextControl = await this.SetupRichTextScenarioAsync(withNoValue: true);

            Assert.That(richTextControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IRichText.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.GenerateRandomText();
            var richTextControl = await this.SetupRichTextScenarioAsync(withValue: expectedValue);

            Assert.That(richTextControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IRichText.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var richTextControl = await this.SetupRichTextScenarioAsync(withNoValue: true);
            var expectedValue = this.GenerateRandomText();

            await richTextControl.SetValueAsync(expectedValue);

            Assert.That(richTextControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IRichText.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var richTextControl = await this.SetupRichTextScenarioAsync();
            var expectedValue = this.GenerateRandomText();

            await richTextControl.SetValueAsync(expectedValue);

            Assert.That(richTextControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        private string GenerateRandomText()
        {
            return this.faker.Lorem.Sentence(100);
        }

        /// <summary>
        /// Sets up a control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IRichText"/>.</returns>
        private async Task<IRichText> SetupRichTextScenarioAsync(string? withValue = null, bool withNoValue = false)
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

            return recordPage.Form.GetField<IRichText>("TBC").Control;
        }
    }
}