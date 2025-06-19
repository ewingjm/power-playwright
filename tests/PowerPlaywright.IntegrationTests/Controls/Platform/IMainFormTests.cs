namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IMainForm"/> platform control.
    /// </summary>
    public class IMainFormTests : IntegrationTests
    {
        /// <summary>
        /// Tests that <see cref="IMainForm.OpenTabAsync(string)"/> opens a tab when the tab exists.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenTabAsync_TabExists_ChangesActiveTab()
        {
            var form = await this.SetupFormScenarioAsync();

            await form.OpenTabAsync(pp_Record.Forms.Information.Tabs.TabB);

            Assert.That(form.GetActiveTabAsync, Is.EqualTo(pp_Record.Forms.Information.Tabs.TabB), "Active tab should match the opened tab.");
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.GetActiveTabAsync"/> returns the current active tab.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetActiveTabAsync_Always_ReturnsCurrentTab()
        {
            var form = await this.SetupFormScenarioAsync();

            Assert.That(form.GetActiveTabAsync, Is.EqualTo(pp_Record.Forms.Information.Tabs.TabA), "Active tab should match the current tab.");
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.GetAllTabsAsync()"/> always retusn a collection of all visible tabs.
        /// </summary>
        /// <returns>All tabs visible on the form.</returns>
        [Test]
        public async Task GetAllTabsAsync_Always_ReturnsAllVisibleTabs()
        {
            var form = await this.SetupFormScenarioAsync();

            Assert.That(form.GetAllTabsAsync, Is.EqualTo(new[] { pp_Record.Forms.Information.Tabs.TabA, pp_Record.Forms.Information.Tabs.TabB, pp_Record.Forms.Information.Tabs.Related }));
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.IsDisabledAsync"/> returns false for an active record.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsDisabledAsync_ActiveRecord_ReturnsFalse()
        {
            var form = await this.SetupFormScenarioAsync(withDisabledRecord: false);

            Assert.That(form.IsDisabledAsync, Is.False, "Form should not be disabled for an active record.");
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.IsDisabledAsync"/> returns true for an inactive record.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsDisabledAsync_InactiveRecord_ReturnsTrue()
        {
            var form = await this.SetupFormScenarioAsync(withDisabledRecord: true);

            Assert.That(form.IsDisabledAsync, Is.True, "Form should be disabled for an inactive record.");
        }

        /// <summary>
        /// Sets up a form scenario.
        /// </summary>
        /// <param name="withDisabledRecord">Whether or not to set up the record as disabled (e.g. inactive).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<IMainForm> SetupFormScenarioAsync(bool withDisabledRecord = false)
        {
            var recordfaker = new RecordFaker();

            if (withDisabledRecord)
            {
                recordfaker.RuleFor(r => r.statecode, r => pp_record_statecode.Inactive);
                recordfaker.RuleFor(r => r.statuscode, r => pp_record_statuscode.Inactive);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(recordfaker.Generate());

            await recordPage.Page.WaitForAppIdleAsync();

            return recordPage.Form;
        }
    }
}