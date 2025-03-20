namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IUrlControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class UrlControl : PcfControl, IUrlControl
    {
        private readonly ILocator urlInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLookupControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public UrlControl(string name, IAppPage appPage, IControl parent = null)
            : base(name, appPage, parent)
        {
            this.urlInput = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            return await this.urlInput.InputValueOrNullAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.urlInput.FillAsync(value);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.FieldControls.UrlControl|{this.Name}.fieldControl|']");
        }
    }
}