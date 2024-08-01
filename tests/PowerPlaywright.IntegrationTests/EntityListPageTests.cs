namespace PowerPlaywright.IntegrationTests
{
    using System.Text.RegularExpressions;
    using PowerPlaywright.Model.Controls.Pcf.Classes;
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
            this.listPage = await this.ModelDrivenApp
                .LoginAsync<IEntityListPage>(Configuration.Url, "pp_UserInterfaceDemo", Configuration.Users.First().Username, Configuration.Users.First().Password);
        }

        /// <summary>
        /// Calling <see cref="ModelDrivenApp.LoginAsync"/> with valid credentials should login to the app.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Grid_OpenRecordAsync_OpensRecordInGrid()
        {
            var entityFormPage = await this.listPage.Grid.OpenRecordAsync(0);

            await this.Expect(entityFormPage.Page).ToHaveURLAsync(EntityFormPageRegex());
        }

        [GeneratedRegex(@".*pagetype=entityrecord.*")]
        private static partial Regex EntityFormPageRegex();
    }
}
