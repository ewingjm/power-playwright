namespace PowerPlaywright.IntegrationTests.Pages
{
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;
    using PowerPlaywright.TestApp.Model.App;

    /// <summary>
    /// Tests for the <see cref="DashboardPage"/> class.
    /// </summary>
    public class DashboardPageTests : IntegrationTests
    {
        private IDashboardPage? dashboardPage;

        /// <summary>
        /// Sets up the list page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            var homePage = await this.LoginAsync();

            this.dashboardPage = await homePage.SiteMap.OpenPageAsync<IDashboardPage>(
                UserInterfaceDemo.SiteMap.AreaA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.Dashboard);
        }

        /// <summary>
        /// Tests that <see cref="IModelDrivenAppPage.Search"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Search_Always_Exists()
        {
            await this.Expect(this.dashboardPage!.Search.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that <see cref="ModelDrivenAppPage.SiteMap"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SiteMap_Always_Exists()
        {
            await this.Expect(this.dashboardPage!.SiteMap.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that the <see cref="AppPage.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.dashboardPage!.Page, Is.Not.Null);
        }
    }
}