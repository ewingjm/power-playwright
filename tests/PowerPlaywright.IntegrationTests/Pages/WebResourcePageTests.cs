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
        /// Tests that <see cref="IWebResource.SiteMap"/> always exists on the page.
        /// </summary>
        [Test]
        public void SiteMap_Always_Exists()
        {
            Assert.That(this.webResourcePage!.SiteMap.Container.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="IWebResource.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.webResourcePage!.Page, Is.Not.Null);
        }
    }
}