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
        /// Tests that <see cref="IDashboardPage.SiteMap"/> always exists on the page.
        /// </summary>
        [Test]
        public void SiteMap_Always_Exists()
        {
            Assert.That(this.dashboardPage!.SiteMap.Container.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="IDashboardPage.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.dashboardPage!.Page, Is.Not.Null);
        }
    }
}