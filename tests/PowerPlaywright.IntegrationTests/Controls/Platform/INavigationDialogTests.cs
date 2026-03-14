namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Pages;
    using PowerPlaywright.Strategies.Controls.Platform;

    /// <summary>
    /// Tests for the <see cref="INavigationDialog"/> control.
    /// </summary>
    public class INavigationDialogTests : IntegrationTests
    {
        private EntityListPage appPage;

        private Faker faker;

        /// <summary>
        /// Sets up the test dependencies.
        /// </summary>
        [SetUp]
        public async Task Setup()
        {
            this.faker = new Faker("en_GB");

            this.appPage = (EntityListPage)await this.LoginAsync();
        }

        /// <summary>
        /// Tests that <see cref="INavigationDialog.IsVisibleAsync"/> returns true when the dialog is visible.
        /// </summary>
        /// <param name="pageContentType">The model-driven app page content type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(typeof(IEntityRecordPageContent))]
        [TestCase(typeof(ICustomPageContent))]
        [TestCase(typeof(IWebResourcePageContent))]
        [TestCase(typeof(IEntityListPageContent))]
        public async Task IsVisibleAsync_DialogIsVisible_ReturnsTrue(Type pageContentType)
        {
            var dialog = await this.SetupDialogScenarioAsync(pageContentType);

            Assert.That(dialog.IsVisibleAsync, Is.EqualTo(true));
        }

        /// <summary>
        /// Tests that the <see cref="INavigationDialog.Content"/> always not null.
        /// </summary>
        /// <param name="pageContentType">The model-driven app page content type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(typeof(IEntityRecordPageContent))]
        [TestCase(typeof(ICustomPageContent))]
        [TestCase(typeof(IWebResourcePageContent))]
        [TestCase(typeof(IEntityListPageContent))]
        public async Task PageContent_Always_NotNull(Type pageContentType)
        {
            var dialog = await this.SetupDialogScenarioAsync(pageContentType);

            Assert.That(dialog.Content, Is.Not.Null);
        }

        /// <summary>
        /// Tests that <see cref="INavigationDialog.CloseAsync"/> closes the dialog when it is visible..
        /// </summary>
        /// <param name="pageContentType">The page content type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(typeof(IEntityRecordPageContent))]
        [TestCase(typeof(ICustomPageContent))]
        [TestCase(typeof(IWebResourcePageContent))]
        [TestCase(typeof(IEntityListPageContent))]
        public async Task CloseAsync_DialogOpen_Closes(Type pageContentType)
        {
            var dialog = await this.SetupDialogScenarioAsync(pageContentType);

            await dialog.CloseAsync();
            if (typeof(EntityRecordPageContent) == pageContentType)
            {
                await this.appPage.ConfirmDialog.CancelAsync();
            }

            await this.Expect(dialog.Container).Not.ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that <see cref="INavigationDialog.GetTitleAsync"/> returns the dialog title.
        /// </summary>
        /// <param name="pageContentType">The page content type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(typeof(IEntityRecordPageContent))]
        [TestCase(typeof(ICustomPageContent))]
        [TestCase(typeof(IWebResourcePageContent))]
        [TestCase(typeof(IEntityListPageContent))]
        public async Task GetTitleAsync_DialogOpen_ReturnsTitle(Type pageContentType)
        {
            var expectedTitle = "Custom Dialog";
            var dialog = await this.SetupDialogScenarioAsync(pageContentType);

            Assert.That(dialog.GetTitleAsync, Is.EqualTo(expectedTitle));
        }

        private async Task<INavigationDialog<IModelDrivenAppPageContent>> SetupDialogScenarioAsync(Type pageContentType)
        {
            var commandModelDialogs = "Model Dialogs";
            var subCommandEntityRecord = "Entity Record";
            var subCommandCustomPage = "Custom Page";
            var subCommandWebResource = "WebResource";
            var subCommandEntityList = "Entity List";

            switch (pageContentType)
            {
                case Type t when t == typeof(IEntityRecordPageContent):
                    return await this.appPage.DataSet.CommandBar
                        .ClickCommandWithDialogAsync<INavigationDialog<IEntityRecordPageContent>>(commandModelDialogs, subCommandEntityRecord);
                case Type t when t == typeof(ICustomPageContent):
                    return await this.appPage.DataSet.CommandBar
                        .ClickCommandWithDialogAsync<INavigationDialog<ICustomPageContent>>(commandModelDialogs, subCommandCustomPage);
                case Type t when t == typeof(IWebResourcePageContent):
                    return await this.appPage.DataSet.CommandBar
                        .ClickCommandWithDialogAsync<INavigationDialog<IWebResourcePageContent>>(commandModelDialogs, subCommandWebResource);
                case Type t when t == typeof(IEntityListPageContent):
                    return await this.appPage.DataSet.CommandBar
                        .ClickCommandWithDialogAsync<INavigationDialog<IWebResourcePageContent>>(commandModelDialogs, subCommandEntityList);
                default:
                    throw new InvalidOperationException($"Unsupported page content type '{pageContentType}'.");
            }
        }
    }
}