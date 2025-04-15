namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IWholeNumber"/> PCF control class.
    /// </summary>
    public class IWholeNumberTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the whole number control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumber.SetValueAsync(int)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var wholeNumberValue = this.faker.Random.Int(int.MinValue, int.MaxValue);
            var wholeNumberControl = await this.SetupWholeNumberScenarioAsync();

            await wholeNumberControl.SetValueAsync(wholeNumberValue);

            Assert.That(wholeNumberControl.GetValueAsync, Is.EqualTo(wholeNumberValue));
        }

        /// <summary>
        /// Tests that <see cref="IWholeNumber.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var wholeNumberControl = await this.SetupWholeNumberScenarioAsync(withWholeNumber: false);

            Assert.That(wholeNumberControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a Whole Number control scenario for testing by creating a record with a specified or generated Whole Number.
        /// </summary>
        /// <param name="withWholeNumber">An optional Whole Number Value to set in the record. If null, a random Whole Number will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IWholeNumber"/>.</returns>
        private async Task<IWholeNumber> SetupWholeNumberScenarioAsync(bool withWholeNumber = true)
        {
            var record = new RecordFaker();

            if (!withWholeNumber)
            {
                record.Ignore(p => p.pp_wholenumbernone);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetControl<IWholeNumber>(nameof(pp_Record.pp_wholenumbernone));
        }
    }
}