namespace PowerPlaywright.IntegrationTests.Pages
{
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="EntityListPage"/> class.
    /// </summary>
    public class EntityRecordPageTests : IntegrationTests
    {
        private IEntityRecordPage recordPage;

        /// <summary>
        /// Sets up the record page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task Setup()
        {
            var record = new RecordFaker().Generate();

            using (var client = this.GetServiceClient())
            {
                await client.CreateAsync(record);
            }

            this.recordPage = await (await this.LoginAsync()).NavigateToRecordAsync(record.LogicalName, record.Id);
        }

        /// <summary>
        /// Tests that <see cref="IEntityRecordPage.Form"/> opens the provided tab when <see cref="IMainFormControl.OpenTabAsync(string)"/> is called with a valid tab.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenTabAsync_TabVisible_OpensTab()
        {
            await this.recordPage.Form.OpenTabAsync(pp_Record.Forms.Information.Tabs.TabB);

            var activeTab = await this.recordPage.Form.GetActiveTabAsync();

            Assert.That(activeTab, Is.EqualTo(pp_Record.Forms.Information.Tabs.TabB));
        }

        /// <summary>
        /// Tests that <see cref="IEntityRecordPage.Form"/> can open a tab when <see cref="IMainFormControl.OpenTabAsync(string)"/> is called.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetActiveTabAsync_TabIsActive_ReturnsActiveTab()
        {
            var activeTab = await this.recordPage.Form.GetActiveTabAsync();

            Assert.That(activeTab, Is.EqualTo(pp_Record.Forms.Information.Tabs.TabA));
        }

        /// <summary>
        /// Tests that <see cref="IEntityRecordPage.Form"/> returns a control when <see cref="IMainFormControl.GetControl{TControl}(string)"/> is called with a non-null or empty control name.
        /// </summary>
        [Test]
        public void Form_GetControl_ReturnsControl()
        {
            var gridControl = this.recordPage.Form.GetControl<IReadOnlyGrid>(pp_Record.Forms.Information.RelatedRecordsSubgrid);

            Assert.That(gridControl, Is.Not.Null);
        }
    }
}
