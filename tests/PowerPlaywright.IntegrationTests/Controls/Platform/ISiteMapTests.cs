namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Text.RegularExpressions;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Controls.Platform;
    using PowerPlaywright.TestApp.Model.App;

    /// <summary>
    /// Tests for the <see cref="SiteMap"/> control.
    /// </summary>
    public partial class ISiteMapTests : IntegrationTests
    {
        private ISiteMap siteMapControl;

        /// <summary>
        /// Sets up the site map.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            this.siteMapControl = (await this.LoginAsync()).SiteMap;
        }

        /// <summary>
        /// Tests that <see cref="ISiteMap.OpenPageAsync{TPage}(string, string, string)"/> opens a page correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenPageAsync_PageExists_OpensPage()
        {
            var page = await this.siteMapControl.OpenPageAsync<IEntityListPage>(
                UserInterfaceDemo.SiteMap.AreaA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords);

            await this.Expect(page.Page).ToHaveURLAsync(RelatedRecordListUrlRegex());
        }

        /// <summary>
        /// Tests that <see cref="ISiteMap.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the page doesn't exist.
        /// </summary>
        [Test]
        public void OpenPageAsync_PageDoesNotExist_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.siteMapControl.OpenPageAsync<IEntityListPage>(
                UserInterfaceDemo.SiteMap.AreaA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName,
                "This page doesn't exist"));
        }

        /// <summary>
        /// Tests that <see cref="ISiteMap.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the group doesn't exist.
        /// </summary>
        [Test]
        public void OpenPageAsync_GroupDoesNotExist_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.siteMapControl.OpenPageAsync<IEntityListPage>(
                UserInterfaceDemo.SiteMap.AreaA.DisplayName,
                "This group doesn't exist",
                UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords));
        }

        /// <summary>
        /// Tests that <see cref="ISiteMap.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the group doesn't exist.
        /// </summary>
        [Test]
        public void OpenPageAsync_AreaDoesNotExist_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.siteMapControl.OpenPageAsync<IEntityListPage>(
                "This area doesn't exist",
                UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords));
        }

        /// <summary>
        /// Tests that <see cref="ISiteMap.GetAreasAsync(string)"/> returns the areas in the site map.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetAreasAsync_Always_ReturnsAreas()
        {
            var expectedAreas = new[]
            {
                UserInterfaceDemo.SiteMap.AreaA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaB.DisplayName,
            };

            var areas = await this.siteMapControl.GetAreasAsync();

            Assert.That(areas, Is.EquivalentTo(expectedAreas));
        }

        /// <summary>
        /// Tests that <see cref="ISiteMap.GetGroupsAsync(string)"/> returns the groups when the areas exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetGroupsAsync_AreaExists_ReturnsGroups()
        {
            var expectedGroups = new[]
            {
                UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.DisplayName,
            };

            var groups = await this.siteMapControl.GetGroupsAsync(UserInterfaceDemo.SiteMap.AreaA.DisplayName);

            Assert.That(groups, Is.EquivalentTo(expectedGroups));
        }

        /// <summary>
        /// Tests that <see cref="ISiteMap.GetPagesAsync(string, string)"/> returns the pages in the area when only the area is passed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetPagesAsync_AreaOnly_ReturnsAllPagesInArea()
        {
            var expectedPages = new[]
            {
                UserInterfaceDemo.SiteMap.AreaA.GroupA.Records,
                UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.Dashboard,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.WebResource,
                UserInterfaceDemo.SiteMap.AreaA.GroupB.Custom,
            };

            var pages = await this.siteMapControl.GetPagesAsync(UserInterfaceDemo.SiteMap.AreaA.DisplayName);

            Assert.That(pages, Is.EquivalentTo(expectedPages));
        }

        /// <summary>
        /// Tests that <see cref="ISiteMap.GetPagesAsync(string, string)"/> returns all the pages in the group when the area and group is passed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetPagesAsync_AreaAndGroup_ReturnsAllPagesInAreaAndGroup()
        {
            var expectedPages = new[]
            {
                UserInterfaceDemo.SiteMap.AreaA.GroupA.Records,
                UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords,
            };

            var pages = await this.siteMapControl.GetPagesAsync(UserInterfaceDemo.SiteMap.AreaA.DisplayName, UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName);

            Assert.That(pages, Is.EquivalentTo(expectedPages));
        }

        [GeneratedRegex(".*pagetype=entitylist&etn=pp_relatedrecord.*")]
        private static partial Regex RelatedRecordListUrlRegex();
    }
}