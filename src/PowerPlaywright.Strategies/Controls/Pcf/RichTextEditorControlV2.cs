namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IMultiLineRichTextControlV2"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class RichTextEditorControlV2 : PcfControlInternal, IMultiLineRichTextControlV2
    {
        private readonly ILocator richTextArea;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextEditorControlV2"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public RichTextEditorControlV2(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.richTextArea = this.Container.GetByRole(AriaRole.Textbox);
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var innerHtml = await this.richTextArea.InnerHTMLAsync();

            string textOnly = Regex.Replace(innerHtml, "<.*?>", string.Empty).Trim();

            return string.IsNullOrEmpty(textOnly) ? null : WebUtility.HtmlDecode(innerHtml);
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.richTextArea.FocusAsync();
            await this.richTextArea.FillAsync(string.Empty);

            if (value != null)
            {
                await this.richTextArea.FillAsync(value);
            }

            await this.Parent.Container.ClickAndWaitForAppIdleAsync();
        }
    }
}