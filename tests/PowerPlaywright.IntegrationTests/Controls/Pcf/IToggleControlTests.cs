namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IToggleControl"/> control class.
    /// </summary>
    public partial class IToggleControlTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the toggle control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }

        /// <summary>
        /// Tests that <see cref="IToggleControl.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Random.Bool();
            var toggleControl = await this.SetupYesNoScenarioAsync(withValue: expectedValue);

            Assert.That(toggleControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IToggleControl.SetValueAsync(bool)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var toggleControl = await this.SetupYesNoScenarioAsync();
            var expectedValue = this.faker.Random.Bool();

            await toggleControl.SetValueAsync(expectedValue);

            Assert.That(toggleControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        private async Task<IToggleControl> SetupYesNoScenarioAsync(bool? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_yesno, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_yesno, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IToggleControl>($"{nameof(pp_Record.pp_yesno)}1").Control;
        }
    }
}