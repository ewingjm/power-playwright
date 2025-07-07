namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IMainFormHeader"/> platform control.
    /// </summary>
    public class IMainFormHeaderTests : IntegrationTests
    {
        /// <summary>
        /// Tests that <see cref="IMainFormHeader.GetFieldsAsync"/> always returns fields on the header (excludes main form fields).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetFieldsAsync_Always_ReturnsHeaderFormFields()
        {
            var form = await this.SetupFormHeaderScenarioAsync();
            await using var header = await form.ExpandHeaderAsync();

            var fields = await header.GetFieldsAsync();

            Assert.That(fields.ToList(), Has.Count.EqualTo(22).And.All.Property(nameof(IField.Location)).EqualTo(FieldLocation.Header));
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.ExpandHeaderAsync"/> always expands the header on the main form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ExpandHeader_Always_ExpandsHeader()
        {
            var form = await this.SetupFormHeaderScenarioAsync();
            await using var header = await form.ExpandHeaderAsync();

            var fields = await header.GetFieldsAsync();

            Assert.That(await Task.WhenAll(fields.Select(f => f.IsVisibleAsync())), Is.All.True);
        }

        /// <summary>
        /// Tests that <see cref="IMainForm.CollapseHeaderAsync"/> always collapses the header on the main form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task DisposeAsync_Always_CollapsesHeader()
        {
            var form = await this.SetupFormHeaderScenarioAsync();
            var header = await form.ExpandHeaderAsync();
            var fields = await header.GetFieldsAsync();

            await header.DisposeAsync();

            Assert.That(await Task.WhenAll(fields.Select(f => f.IsVisibleAsync())), Is.All.False);
        }

        /// <summary>
        /// Sets up a form header scenario.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<IMainForm> SetupFormHeaderScenarioAsync()
        {
            var recordPage = await this.LoginAndNavigateToRecordAsync(new RecordFaker().Generate());

            return recordPage.Form;
        }
    }
}