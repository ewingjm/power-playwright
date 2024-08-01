namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Model;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Model.Controls.Pcf;
    using PowerPlaywright.Model.Controls.Pcf.Attributes;
    using PowerPlaywright.Pages;

    /// <summary>
    /// A control strategy for the <see cref="IPcfGridControl"/>.
    /// </summary>
    [PcfControlStrategy(1, 1, 148)]
    public class PcfGridControl : Control, IPcfGridControl
    {
        private readonly string name;
        private readonly IPageFactory pageFactory;
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator rowsContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfGridControl"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public PcfGridControl(IPage page, string name, IControl parent, IPageFactory pageFactory, ILogger<PcfGridControl> logger = null)
            : base(page, parent)
        {
            this.name = name;
            this.pageFactory = pageFactory;
            this.logger = logger;

            this.rowsContainer = this.Container.Locator("div.ag-center-cols-viewport");
        }

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> OpenRecordAsync(int index)
        {
            await this.GetRow(index).DblClickAsync();

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }

        /// <inheritdoc/>
        protected override ILocator GetContainer()
        {
            return this.Parent?.Container.Locator(this.GetContainerSelector()) ?? this.Page.Locator(this.GetContainerSelector());
        }

        private string GetContainerSelector()
        {
            return $"div[data-lp-id*='MscrmControls.Grid.PCFGridControl|{this.name}']";
        }

        private ILocator GetRow(int index)
        {
            return this.rowsContainer.Locator($"div[role='row'][row-index='{index}']");
        }
    }
}