namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Tests for the <see cref="IConfirmDialog"/> control.
    /// </summary>
    public class IConfirmDialogTests : IntegrationTests
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
        /// Tests that <see cref="IConfirmDialog.GetTitleAsync"/> returns the title when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetTitleAsync_DialogIsVisible_ReturnsTitle()
        {
            var expectedTitle = this.faker.Lorem.Word();
            var appPage = await this.SetupConfirmDialogScenarioAsync(withTitle: expectedTitle);

            Assert.That(appPage.ConfirmDialog.GetTitleAsync, Is.EqualTo(expectedTitle));
        }

        /// <summary>
        /// Tests that <see cref="IConfirmDialog.GetSubtitleAsync"/> returns the subtitle when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetSubtitleAsync_DialogIsVisible_ReturnsSubtitle()
        {
            var expectedSubtitle = this.faker.Lorem.Word();
            var appPage = await this.SetupConfirmDialogScenarioAsync(withSubtitle: expectedSubtitle);

            Assert.That(appPage.ConfirmDialog.GetSubtitleAsync, Is.EqualTo(expectedSubtitle));
        }

        /// <summary>
        /// Tests that <see cref="IConfirmDialog.GetTextAsync"/> returns the text when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetTextAsync_DialogIsVisible_ReturnsText()
        {
            var expectedText = this.faker.Lorem.Sentences(2);
            var appPage = await this.SetupConfirmDialogScenarioAsync(withText: expectedText);

            Assert.That(appPage.ConfirmDialog.GetTextAsync, Is.EqualTo(expectedText));
        }

        /// <summary>
        /// Tests that <see cref="IConfirmDialog.ConfirmAsync"/> confirms the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ConfirmAsync_DialogIsVisible_Confirms()
        {
            var appPage = await this.SetupConfirmDialogScenarioAsync();

            await appPage.ConfirmDialog.ConfirmAsync();

            Assert.That(await appPage.Page.EvaluateAsync<bool>("window.confirmClicked"), Is.EqualTo(true));
        }

        /// <summary>
        /// Tests that <see cref="IConfirmDialog.CancelAsync"/> cancels the dialog when it is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ConfirmAsync_DialogIsVisible_Cancels()
        {
            var appPage = await this.SetupConfirmDialogScenarioAsync();

            await appPage.ConfirmDialog.CancelAsync();

            Assert.That(await appPage.Page.EvaluateAsync<bool>("window.confirmClicked"), Is.EqualTo(false));
        }

        /// <summary>
        /// Tests that <see cref="IConfirmDialog.IsVisibleAsync"/> returns true when the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_DialogIsVisible_ReturnsTrue()
        {
            var appPage = await this.SetupConfirmDialogScenarioAsync();

            Assert.That(appPage.ConfirmDialog.IsVisibleAsync, Is.EqualTo(true));
        }

        /// <summary>
        /// Sets up a confirm dialog scenario.
        /// </summary>
        /// <param name="withTitle">An optional title. It will be empty if not supplied.</param>
        /// <param name="withSubtitle">An optional subtitle. It will be empty if not supplied.</param>
        /// <param name="withText">An optional body of text. It will be randomly generated if not supplied.</param>
        private async Task<IModelDrivenAppPage> SetupConfirmDialogScenarioAsync(string? withTitle = null, string? withSubtitle = null, string? withText = null)
        {
            var appPage = await this.LoginAsync();

            var confirmStrings = new Dictionary<string, string>()
            {
                ["text"] = withText ?? this.faker.Lorem.Sentence(),
            };

            if (withTitle != null)
            {
                confirmStrings.Add("title", withTitle);
            }

            if (withSubtitle != null)
            {
                confirmStrings.Add("subtitle", withSubtitle);
            }

            await appPage.Page.EvaluateAsync("confirmStrings => { Xrm.Navigation.openConfirmDialog(confirmStrings).then(success => window.confirmClicked = success.confirmed) } ", confirmStrings);

            return appPage;
        }
    }
}
