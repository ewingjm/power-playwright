namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System;
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IModelDrivenAppPageContent"/> control.
    /// </summary>
    public class INavigationDialogTests : IntegrationTests
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
        /// Tests that the <see cref="IModelDrivenAppPageContent.Container"/> always exists on the page.
        /// </summary>
        /// <param name="pageContentType">The model-driven app page content type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(typeof(IEntityRecordPageContent))]
        [TestCase(typeof(IEntityListPageContent))]
        public async Task PageContent_Always_Exists(Type pageContentType)
        {
            var pageContent = await this.SetupNavigationScenario(pageContentType);

            await this.Expect(pageContent.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that the <see cref="IModelDrivenAppPageContent.Container"/> always not null.
        /// </summary>
        /// <param name="pageContentType">The model-driven app page content type.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(typeof(IEntityRecordPageContent))]
        [TestCase(typeof(IEntityListPageContent))]
        public async Task PageContent_Always_NotNull(Type pageContentType)
        {
            var pageContent = await this.SetupNavigationScenario(pageContentType);

            Assert.That(pageContent.Container, Is.Not.Null);
        }

        private async Task<IModelDrivenAppPageContent> SetupNavigationScenario(Type pageContentType)
        {
            switch (pageContentType)
            {
                case Type t when t == typeof(IEntityRecordPageContent):
                    var recordPage = await this.LoginAndNavigateToRecordAsync(new RecordFaker().Generate());
                    return recordPage.Content;
                case Type t when t == typeof(IEntityListPageContent):
                    var listPage = (IEntityListPage)await this.LoginAsync();
                    return listPage.Content;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}