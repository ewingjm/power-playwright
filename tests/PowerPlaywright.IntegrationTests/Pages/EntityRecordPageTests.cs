namespace PowerPlaywright.IntegrationTests.Pages
{
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="EntityListPage"/> class.
    /// </summary>
    public class EntityRecordPageTests : IntegrationTests
    {
        private IEntityRecordPage? recordPage;

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

            this.recordPage = await (await this.LoginAsync()).ClientApi.NavigateToRecordAsync(record.LogicalName, record.Id);
        }

        /// <summary>
        /// Tests that <see cref="IEntityRecordPage.Form"/> always exists on the page.
        /// </summary>
        [Test]
        public void Form_Always_Exists()
        {
            Assert.That(this.recordPage!.Form.Container.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IEntityRecordPage.SiteMap"/> always exists on the page.
        /// </summary>
        [Test]
        public void SiteMap_Always_Exists()
        {
            Assert.That(this.recordPage!.SiteMap.Container.IsVisibleAsync(), Is.True);
        }

        /// <summary>
        /// Tests that the <see cref="IEntityRecordPage.Page"/> property is always not null.
        /// </summary>
        [Test]
        public void Page_Always_NotNull()
        {
            Assert.That(this.recordPage!.Page, Is.Not.Null);
        }
    }
}