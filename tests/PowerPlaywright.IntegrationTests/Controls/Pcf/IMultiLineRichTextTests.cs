namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Strategies.Controls.Pcf;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Tests the <see cref="IMultiLineRichTextTests"/> PCF control class.
    /// </summary>
    public class IMultiLineRichTextTests : IntegrationTests
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
        /// Tests that <see cref="IMultiLineRichText.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var richTextControl = await this.SetupRichTextScenarioAsync(withNoValue: true);

            Assert.That(richTextControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IMultiLineRichText.GetValueAsync"/> returns the value when the value has been set.
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
        /// Tests that <see cref="IMultiLineRichText.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var richTextControl = await this.SetupRichTextScenarioAsync(withNoValue: true);
            var expectedValue = this.GenerateRandomText();

            await richTextControl.SetValueAsync(expectedValue);

            var actualValue = await richTextControl.GetValueAsync();

            Assert.That(this.NormalizeLineEndings(actualValue), Is.EqualTo(this.NormalizeLineEndings(expectedValue)));
        }

        /// <summary>
        /// Tests that <see cref="IMultiLineRichText.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var richTextControl = await this.SetupRichTextScenarioAsync();
            var expectedValue = this.GenerateRandomText();

            await richTextControl.SetValueAsync(expectedValue);
            var actualValue = await richTextControl.GetValueAsync();

            Assert.That(this.NormalizeLineEndings(actualValue), Is.EqualTo(this.NormalizeLineEndings(expectedValue)));
        }

        private string GenerateRandomText()
        {
            return this.faker.Lorem.Lines(2);
        }

        /// <summary>
        /// Removes formatiing supporting so running on all platforms does not cause test failures due to differences in line endings or multiple line breaks.
        /// </summary>
        /// <param name="value">The string to normalise the endings on.</param>
        /// <returns>A <see cref="string"/> with endings replaced.</returns>
        private string NormalizeLineEndings(string value)
        {
            const string ending = "\n";
            value = value.ReplaceLineEndings(ending);
            return Regex.Replace(value, $@"{ending}+", ending);
        }

        /// <summary>
        /// Sets up a control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IMultiLineRichText"/>.</returns>
        private async Task<IMultiLineRichText> SetupRichTextScenarioAsync(string? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_multilinerichtext, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_multilinerichtext, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetField<IMultiLineRichText>(nameof(pp_Record.pp_multilinerichtext)).Control;
        }
    }
}