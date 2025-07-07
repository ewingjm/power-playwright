namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Tests for the <see cref="IAlertDialog"/> control.
    /// </summary>
    public class IAlertDialogTests : IntegrationTests
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
        /// Tests that <see cref="IAlertDialog.GetTitleAsync"/> returns the title when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetTitleAsync_DialogIsVisible_ReturnsTitle()
        {
            var expectedTitle = this.faker.Lorem.Word();
            var appPage = await this.SetupAlertDialogScenarioAsync(withTitle: expectedTitle);

            Assert.That(appPage.AlertDialog.GetTitleAsync, Is.EqualTo(expectedTitle));
        }

        /// <summary>
        /// Tests that <see cref="IAlertDialog.GetTextAsync"/> returns the text when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetTextAsync_DialogIsVisible_ReturnsText()
        {
            var expectedText = this.faker.Lorem.Sentences(2);
            var appPage = await this.SetupAlertDialogScenarioAsync(withText: expectedText);

            Assert.That(appPage.AlertDialog.GetTextAsync, Is.EqualTo(expectedText));
        }

        /// <summary>
        /// Tests that <see cref="IAlertDialog.ConfirmAsync"/> confirms the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ConfirmAsync_DialogIsVisible_Confirms()
        {
            var appPage = await this.SetupAlertDialogScenarioAsync();

            await appPage.AlertDialog.ConfirmAsync();

            Assert.That(await appPage.Page.EvaluateAsync<bool>("window.confirmClicked"), Is.EqualTo(true));
        }

        /// <summary>
        /// Tests that <see cref="IAlertDialog.IsVisibleAsync"/> returns true when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_DialogIsVisible_ReturnsTrue()
        {
            var appPage = await this.SetupAlertDialogScenarioAsync();

            Assert.That(appPage.AlertDialog.IsVisibleAsync, Is.EqualTo(true));
        }

        /// <summary>
        /// Sets up an alert dialog scenario.
        /// </summary>
        /// <param name="withTitle">An optional title. It will be empty if not supplied.</param>
        /// <param name="withText">An optional body of text. It will be randomly generated if not supplied.</param>
        private async Task<IModelDrivenAppPage> SetupAlertDialogScenarioAsync(string? withTitle = null, string? withText = null)
        {
            var appPage = await this.LoginAsync();

            var alertStrings = new Dictionary<string, string>()
            {
                ["text"] = withText ?? this.faker.Lorem.Sentence(),
            };

            if (withTitle != null)
            {
                alertStrings.Add("title", withTitle);
            }

            await appPage.Page.EvaluateAsync("alertStrings => { Xrm.Navigation.openAlertDialog(alertStrings).then(success => window.confirmClicked = true) } ", alertStrings);

            return appPage;
        }
    }
}
