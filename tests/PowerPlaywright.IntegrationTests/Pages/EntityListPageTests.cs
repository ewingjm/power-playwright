namespace PowerPlaywright.IntegrationTests.Pages
{
    using System.Text.RegularExpressions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;

    /// <summary>
    /// Tests for the <see cref="EntityListPage"/> class.
    /// </summary>
    public partial class EntityListPageTests : IntegrationTests
    {
        private IEntityListPage listPage;

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
        /// Calling <see cref="ModelDrivenApp.LoginAsync"/> with valid credentials should login to the app.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Grid_OpenRecordAsync_OpensRecordInGrid()
        {
            var recordFormPage = await this.listPage.Grid.OpenRecordAsync(0);

            await this.Expect(recordFormPage.Page).ToHaveURLAsync(EntityFormPageRegex());
        }

        [GeneratedRegex(@".*pagetype=entityrecord.*")]
        private static partial Regex EntityFormPageRegex();
    }
}