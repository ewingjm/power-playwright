namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="ISingleLineUrl"/> PCF control class.
    /// </summary>
    public class ISingleLinePhoneNumberTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the phonenumber control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="ISingleLinePhoneNumber.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var phoneNumber = this.faker.Phone.PhoneNumber();
            var phoneNumberControl = await this.SetupPhoneNumberScenarioAsync();

            await phoneNumberControl.SetValueAsync(phoneNumber);

            Assert.That(phoneNumberControl.GetValueAsync, Is.EqualTo(phoneNumber));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLinePhoneNumber.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var urlControl = await this.SetupPhoneNumberScenarioAsync(withPhoneNumber: string.Empty);

            Assert.That(urlControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a Phone Number control scenario for testing by creating a record with a specified or generated Phone Number.
        /// </summary>
        /// <param name="withPhoneNumber">An optional Phone Number string to set in the record. If null, a random Phone Number will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ISingleLinePhoneNumber"/>.</returns>
        private async Task<ISingleLinePhoneNumber> SetupPhoneNumberScenarioAsync(string? withPhoneNumber = null)
        {
            var record = new RecordFaker();
            record.RuleFor(x => x.pp_singlelineoftextphonenumber, f => withPhoneNumber ?? f.Phone.PhoneNumber());

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetControl<ISingleLinePhoneNumber>(nameof(pp_Record.pp_singlelineoftextphonenumber));
        }
    }
}