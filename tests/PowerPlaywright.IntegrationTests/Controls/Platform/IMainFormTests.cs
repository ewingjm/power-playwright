namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Model;
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
        /// Tests that <see cref="IMainForm.GetFieldsAsync"/> always returns fields on the main form (excludes header fields).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetFieldsAsync_Always_ReturnsMainFormFields()
        {
            var form = await this.SetupFormScenarioAsync();

            var fields = await form.GetFieldsAsync();

            Assert.That(fields.ToList(), Has.Count.AtLeast(28).And.All.Property(nameof(IField.Location)).EqualTo(FieldLocation.Body));
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.GetHeaderFieldsAsync"/> always returns header fields on the main form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetHeaderFieldsAsync_Always_ReturnsMainFormHeaderFields()
        {
            var form = await this.SetupFormScenarioAsync();

            var headerFields = await form.GetHeaderFieldsAsync();

            Assert.That(headerFields.ToList(), Has.Count.EqualTo(22).And.All.Property(nameof(IField.Location)).EqualTo(FieldLocation.Header));
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.ExpandHeaderAsync"/> always expands the header on the main form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ExpandHeader_Always_ExpandsHeader()
        {
            var form = await this.SetupFormScenarioAsync();
            var headerField = form.GetField("header_statecode");

            await form.ExpandHeaderAsync();

            Assert.That(form.IsHeaderExpandedAsync, Is.True);
            Assert.That(headerField.IsVisibleAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.CollapseHeaderAsync"/> always collapses the header on the main form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CloseHeader_Always_CollapsesHeader()
        {
            var form = await this.SetupFormScenarioAsync();
            var headerField = form.GetField("header_statecode");

            await form.CollapseHeaderAsync();

            Assert.That(form.IsHeaderExpandedAsync, Is.False);
            Assert.That(headerField.IsVisibleAsync, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.GetFieldsAsync"/> always returns fields on the main form (excludes header fields).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetFieldsAsync_Always_ReturnsQuickViewFields()
        {
            var form = await this.SetupFormScenarioAsync(withQuickViewRecord: true);

            var fields = await form.GetFieldsAsync();

            Assert.That(fields, Has.Some.Matches<IField>(f => f.Name.StartsWith("QuickViewRelatedRecord.")));
        }

        /// <summary>
        /// Sets up a form scenario.
        /// </summary>
        /// <param name="withDisabledRecord">Whether or not to set up the record as disabled (e.g. inactive).</param>
        /// <param name="withQuickViewRecord">Whether or not to set up a related record that will display a quick view.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<IMainForm> SetupFormScenarioAsync(bool withDisabledRecord = false, bool withQuickViewRecord = false)
        {
            var recordfaker = new RecordFaker();

            if (withDisabledRecord)
            {
                recordfaker.RuleFor(r => r.statecode, r => pp_record_statecode.Inactive);
                recordfaker.RuleFor(r => r.statuscode, r => pp_record_statuscode.Inactive);
            }

            if (withQuickViewRecord)
            {
                recordfaker.RuleFor(r => r.pp_relatedrecord_record, f => new RelatedRecordFaker().Generate());
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(recordfaker.Generate());

            await recordPage.Page.WaitForAppIdleAsync();

            return recordPage.Form;
        }
    }
}