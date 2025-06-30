namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="ISingleLineUrl"/> PCF control class.
    /// </summary>
    public class ISingleLineUrlTests : IntegrationTests
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
        /// Tests that <see cref="ISingleLineUrl.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var singleLineUrlControl = await this.SetupSingleLineUrlScenarioAsync(withNoValue: true);

            Assert.That(singleLineUrlControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineUrl.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Internet.Url();
            var singleLineUrlControl = await this.SetupSingleLineUrlScenarioAsync(withValue: expectedValue);

            Assert.That(singleLineUrlControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineUrl.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var singleLineUrlControl = await this.SetupSingleLineUrlScenarioAsync(withNoValue: true);
            var expectedValue = this.faker.Internet.Url();

            await singleLineUrlControl.SetValueAsync(expectedValue);

            Assert.That(singleLineUrlControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineUrl.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var singleLineUrlControl = await this.SetupSingleLineUrlScenarioAsync();
            var expectedValue = this.faker.Internet.Url();

            await singleLineUrlControl.SetValueAsync(expectedValue);

            Assert.That(singleLineUrlControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ISingleLineUrl"/>.</returns>
        private async Task<ISingleLineUrl> SetupSingleLineUrlScenarioAsync(string? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_singlelineoftexturl, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_singlelineoftexturl, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<ISingleLineUrl>(nameof(pp_Record.pp_singlelineoftexturl)).Control;
        }
    }
}