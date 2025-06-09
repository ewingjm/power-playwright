namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using Azure;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Controls.Platform;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.App;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Tests for the <see cref="GlobalSearch"/> control.
    /// </summary>
    public partial class IGlobalSearchTests : IntegrationTests
    {
        private IGlobalSearch searchControl;

        /// <summary>
        /// Sets up the read-only grid.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            this.searchControl = (await this.LoginAsync()).Search;
        }

        /// <summary>
        /// Tests that <see cref="IGlobalSearch.SearchAsync(string)"/> performs a search correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_Returns_Results()
        {
            //TODO create the record first
            await this.searchControl.SearchAsync("51zw");

            var hasResults = await this.searchControl.HasResultsAsync();

            Assert.That(hasResults, Is.True);
        }

        ///// <summary>
        ///// Tests that <see cref="ISiteMap.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the page doesn't exist.
        ///// </summary>
        //[Test]
        //public void OpenPageAsync_PageDoesNotExist_ThrowsNotFoundException()
        //{
        //    Assert.ThrowsAsync<NotFoundException>(() => this.searchControl.OpenPageAsync<IEntityListPage>(
        //        UserInterfaceDemo.SiteMap.AreaA.DisplayName,
        //        UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName,
        //        "This page doesn't exist"));
        //}

        ///// <summary>
        ///// Tests that <see cref="ISiteMap.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the group doesn't exist.
        ///// </summary>
        //[Test]
        //public void OpenPageAsync_GroupDoesNotExist_ThrowsNotFoundException()
        //{
        //    Assert.ThrowsAsync<NotFoundException>(() => this.searchControl.OpenPageAsync<IEntityListPage>(
        //        UserInterfaceDemo.SiteMap.AreaA.DisplayName,
        //        "This group doesn't exist",
        //        UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords));
        //}

        ///// <summary>
        ///// Tests that <see cref="ISiteMap.OpenPageAsync{TPage}(string, string, string)"/> throws a <see cref="NotFoundException"/> if the group doesn't exist.
        ///// </summary>
        //[Test]
        //public void OpenPageAsync_AreaDoesNotExist_ThrowsNotFoundException()
        //{
        //    Assert.ThrowsAsync<NotFoundException>(() => this.searchControl.OpenPageAsync<IEntityListPage>(
        //        "This area doesn't exist",
        //        UserInterfaceDemo.SiteMap.AreaA.GroupA.DisplayName,
        //        UserInterfaceDemo.SiteMap.AreaA.GroupA.RelatedRecords));
        //}

        //[GeneratedRegex(".*pagetype=entitylist&etn=pp_relatedrecord.*")]
        //private static partial Regex RelatedRecordListUrlRegex();
    }
}