namespace PowerPlaywright.IntegrationTests.Pages
{
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;

    /// <summary>
    /// Tests for the <see cref="EntityListPage"/> class.
    /// </summary>
    public class RecordsPageTests : IntegrationTests
    {
        private RecordsPage recordsPage;

        /// <summary>
        /// Sets up the list page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            var listPage = (IEntityListPage)await this.LoginAsync();
            this.recordsPage = await listPage.SiteMap.OpenPageAsync<RecordsPage>("Area B", "New Group", "Records");
        }

        /// <summary>
        /// Calling <see cref="ModelDrivenApp.LoginAsync"/> with valid credentials should login to the app.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Grid_OpenRecordAsync_OpensRecordInGrid()
        {
            await this.recordsPage.CreateNewRecordAsync("Test name");
        }
    }
}
