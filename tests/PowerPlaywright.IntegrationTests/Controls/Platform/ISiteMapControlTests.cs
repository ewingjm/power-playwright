namespace PowerPlaywright.IntegrationTests.Controls
{
    using System.Text.RegularExpressions;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Controls.Platform;
    using PowerPlaywright.TestApp.Model.App;

    /// <summary>
    /// Tests for the <see cref="SiteMapControl"/> control.
    /// </summary>
    public partial class ISiteMapControlTests : IntegrationTests
    {
        private ISiteMapControl siteMapControl;

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
        /// Tests that <see cref="ISiteMapControl.OpenPageAsync{TPage}(string, string, string)"/> opens a page correctly.
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
        /// Tests that <see cref="ISiteMapControl.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the page doesn't exist.
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
        /// Tests that <see cref="ISiteMapControl.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the group doesn't exist.
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
        /// Tests that <see cref="ISiteMapControl.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the group doesn't exist.
        /// </summary>
        [Test]
        public void OpenPageAsync_AreaDoesNotExist_ThrowsNotFoundException()
        {
            Assert.ThrowsAsync<NotFoundException>(() => this.siteMapControl.OpenPageAsync<IEntityListPage>(
                "This area doesn't exist",
                UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName,
                UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords));
        }

        [GeneratedRegex(".*pagetype=entitylist&etn=pp_relatedrecord.*")]
        private static partial Regex RelatedRecordListUrlRegex();
    }
}