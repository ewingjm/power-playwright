namespace PowerPlaywright.IntegrationTests.Pages
{
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;
    using PowerPlaywright.TestApp.Model.App;

    /// <summary>
    /// Tests for the <see cref="WebResourcePage"/> class.
    /// </summary>
    public class WebResourcePageTests : IntegrationTests
    {
        private IWebResourcePage? webResourcePage;

        /// <summary>
        /// Sets up the record page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            var homePage = await this.LoginAsync();

            this.webResourcePage = await homePage.SiteMap.OpenPageAsync<IWebResourcePage>(
                UserInterfaceDemo.SiteMap.AreaA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.WebResource);
        }

        /// <summary>
        /// Tests that <see cref="IModelDrivenAppPage.Search"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Search_Always_Exists()
        {
            await this.Expect(this.webResourcePage!.Search.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that <see cref="IModelDrivenAppPage.SiteMap"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SiteMap_Always_Exists()
        {
            await this.Expect(this.webResourcePage!.SiteMap.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that the <see cref="IAppPage.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.webResourcePage!.Page, Is.Not.Null);
        }
    }
}