namespace PowerPlaywright.IntegrationTests.Pages
{
    using PowerPlaywright.Pages;
    using PowerPlaywright.TestApp.Model.App;

    /// <summary>
    /// Tests for the <see cref="CustomPage"/> class.
    /// </summary>
    public class CustomPageTests : IntegrationTests
    {
        private CustomPage? customPage;

        /// <summary>
        /// Sets up the list page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            var homePage = await this.LoginAsync();

            this.customPage = await homePage.SiteMap.OpenPageAsync<CustomPage>(
                UserInterfaceDemo.SiteMap.AreaA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.Custom);
        }

        /// <summary>
        /// Tests that <see cref="ICustomPage.SiteMap"/> always exists on the page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SiteMap_Always_Exists()
        {
            await this.Expect(this.customPage!.SiteMap.Container).ToBeVisibleAsync();
        }

        /// <summary>
        /// Tests that the <see cref="ICustomPage.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.customPage!.Page, Is.Not.Null);
        }
    }
}