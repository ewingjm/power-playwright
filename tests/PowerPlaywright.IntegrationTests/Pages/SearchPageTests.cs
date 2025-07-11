namespace PowerPlaywright.IntegrationTests.Pages
{
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;
    using PowerPlaywright.TestApp.Model.App;

    /// <summary>
    /// Tests for the <see cref="SearchPage"/> class.
    /// </summary>
    public class SearchPageTests : IntegrationTests
    {
        private ISearchPage searchPage;

        /// <summary>
        /// Sets up the search page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            this.searchPage = await (await this.LoginAsync()).Search.SearchAsync<ISearchPage>("*");
        }

        /// <summary>
        /// Tests that <see cref="IModelDrivenAppPage.Search"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Search_Always_Exists()
        {
            await this.Expect(this.searchPage!.Search.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that <see cref="IModelDrivenAppPage.SiteMap"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SiteMap_Always_Exists()
        {
            await this.Expect(this.searchPage!.SiteMap.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that the <see cref="IAppPage.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.searchPage!.Page, Is.Not.Null);
        }
    }
}