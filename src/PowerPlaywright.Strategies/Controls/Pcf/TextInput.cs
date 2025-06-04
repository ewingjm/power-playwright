namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Strategies.Extensions;
    using PowerPlaywright.Framework;

    /// <summary>
    /// A control strategy for the <see cref="ITextInput"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class TextInput : PcfControlInternal, ITextInput
    {
        private readonly ILocator textArea;
        private readonly ILocator textInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextInput"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public TextInput(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.textArea = this.Container.Locator("textarea");
            this.textInput = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            if (await textArea.IsVisibleAsync())
            {
                return await this.textArea.InputValueOrNullAsync();
            }

            return await this.textInput.InputValueOrNullAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            if (await textArea.IsVisibleAsync())
            {
                await this.textArea.FillAsync(value);
            }
            else
            {
                await this.textInput.FillAsync(value);
            }
        }
    }
}