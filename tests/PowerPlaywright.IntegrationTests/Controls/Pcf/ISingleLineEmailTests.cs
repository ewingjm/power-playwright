namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="ISingleLineEmail"/> PCF control class.
    /// </summary>
    public class ISingleLineEmailTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the email control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineEmail.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var email = this.faker.Internet.Email();
            var emailControl = await this.SetupEmailScenarioAsync();

            await emailControl.SetValueAsync(email);

            Assert.That(emailControl.GetValueAsync, Is.EqualTo(email));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineEmail.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var emailControl = await this.SetupEmailScenarioAsync(withEmail: string.Empty);

            Assert.That(emailControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a URL control scenario for testing by creating a record with a specified or generated Email.
        /// </summary>
        /// <param name="withEmail">An optional URL string to set in the record. If null, a random Email will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IEmailControl"/>.</returns>
        private async Task<ISingleLineEmail> SetupEmailScenarioAsync(string? withEmail = null)
        {
            var record = new RecordFaker();
            record.RuleFor(x => x.pp_singlelineoftextemail, f => withEmail ?? f.Internet.Email());

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetControl<ISingleLineEmail>(nameof(pp_Record.pp_singlelineoftextemail));
        }
    }
}