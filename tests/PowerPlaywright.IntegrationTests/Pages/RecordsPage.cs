namespace PowerPlaywright.IntegrationTests.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Pages;

    /// <summary>
    /// A page for the Records custom page.
    /// </summary>
    internal class RecordsPage : CustomPage
    {
        private readonly ILocator newRecordButton;
        private readonly ILocator nameInput;
        private readonly ILocator saveRecordButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordsPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="controlFactory">The control factory.</param>
        public RecordsPage(IPage page, IPageFactory pageFactory, IControlFactory controlFactory)
            : base(page, pageFactory, controlFactory)
        {
            this.newRecordButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "New record" });
            this.nameInput = this.Container.GetByRole(AriaRole.Textbox, new LocatorGetByRoleOptions { Name = "Single line of text (text)" });
            this.saveRecordButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Save record" });
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="name">The name of the record.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>.
        public async Task CreateNewRecordAsync(string name)
        {
            await this.newRecordButton.ClickAsync();
            await this.nameInput.FillAsync(name);
            await this.saveRecordButton.ClickAsync();
        }
    }
}
