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
    public class ISingleLineUrlTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the url control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineUrl.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var url = this.faker.Internet.Url();
            var urlControl = await this.SetupUrlScenarioAsync();

            await urlControl.SetValueAsync(url);

            Assert.That(urlControl.GetValueAsync, Is.EqualTo(url));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineUrl.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var urlControl = await this.SetupUrlScenarioAsync(withUrl: string.Empty);

            Assert.That(urlControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a URL control scenario for testing by creating a record with a specified or generated URL.
        /// </summary>
        /// <param name="withUrl">An optional URL string to set in the record. If null, a random URL will be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<ISingleLineUrl> SetupUrlScenarioAsync(string? withUrl = null)
        {
            var record = new RecordFaker();
            record.RuleFor(x => x.pp_singlelineoftexturl, f => withUrl ?? f.Internet.Url());

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetField<ISingleLineUrl>(nameof(pp_Record.pp_singlelineoftexturl)).Control;
        }
    }
}