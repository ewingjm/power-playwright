namespace PowerPlaywright.IntegrationTests.Pages
{
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;

    /// <summary>
    /// Tests for the <see cref="EntityListPage"/> class.
    /// </summary>
    public class EntityListPageTests : IntegrationTests
    {
        private IEntityListPage? listPage;

        /// <summary>
        /// Sets up the list page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            this.listPage = (IEntityListPage)await this.LoginAsync();
        }

        /// <summary>
        /// Tests that <see cref="IEntityListPage.DataSet"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Grid_Always_Exists()
        {
            await this.Expect(this.listPage!.DataSet.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that <see cref="IModelDrivenAppPage.SiteMap"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SiteMap_Always_Exists()
        {
            await this.Expect(this.listPage!.SiteMap.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that the <see cref="IAppPage.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.listPage!.Page, Is.Not.Null);
        }
    }
}