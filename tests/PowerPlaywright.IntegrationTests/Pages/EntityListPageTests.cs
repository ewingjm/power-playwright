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
        /// Tests that <see cref="IEntityListPage.Grid"/> always exists on the page.
        /// </summary>
        [Test]
        public void Grid_Always_Exists()
        {
            Assert.That(this.listPage!.Grid.Container.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IEntityListPage.SiteMap"/> always exists on the page.
        /// </summary>
        [Test]
        public void SiteMap_Always_Exists()
        {
            Assert.That(this.listPage!.SiteMap.Container.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="IEntityListPage.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.listPage!.Page, Is.Not.Null);
        }
    }
}