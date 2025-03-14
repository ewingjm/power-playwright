namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the IUrlControl PCF Control.
    /// </summary>
    internal class IUrlControlTests : IntegrationTests
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
        /// Tests that <see cref="ISingleLineUrl.SetValueAsync(string)"/> returns string when the field has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var urlFaker = this.GeUrlFaker(out var urlValue);
            var urlControl = await this.SetupUrlScenarioAsync(withUrl: urlFaker);

            await urlControl.SetValueAsync(urlValue);

            Assert.That(urlControl.GetValueAsync, Is.EqualTo(urlValue));
        }

        /// <summary>
        /// Tests that <see cref="ISingleLineUrl.GetValueAsync"/> returns empty string when the value has not been set. Checks the value coming back is not a valid Url. Playwright otherwises pases back a placeholder like '---'.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsEmptyString()
        {
            var urlControl = await this.SetupUrlScenarioAsync(withUrl: null, generateNullUrl: true);
            Assert.That(async () => Uri.TryCreate(await urlControl.GetValueAsync(), UriKind.Absolute, out _), Is.False);
        }

        private async Task<IUrlControl> SetupUrlScenarioAsync(Faker<pp_Record>? withUrl = null, bool generateNullUrl = false)
        {
            withUrl ??= new RecordFaker();

            if (generateNullUrl)
            {
                withUrl.RuleFor(x => x.pp_singlelineoftexturl, f => string.Empty);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(withUrl.Generate());

            return recordPage.Form.GetControl<IUrlControl>(nameof(pp_Record.pp_singlelineoftexturl));
        }

        private Faker<pp_Record> GeUrlFaker(out string recordName)
        {
            var url = this.faker.Internet.Url();
            recordName = url;

            return new RecordFaker().RuleFor(r => r.pp_singlelineoftexturl, f => url);
        }
    }
}