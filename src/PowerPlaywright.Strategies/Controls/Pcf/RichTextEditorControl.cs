namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IMultiLineRichTextControlV2"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class RichTextEditorControl : PcfControlInternal, IMultiLineRichTextControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextEditorControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public RichTextEditorControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            var frame = await this.Container
            .Locator("iframe").Last.ElementHandleAsync();

            var contentFrame = await frame.ContentFrameAsync();

            await contentFrame.WaitForFunctionAsync(@"
                () => {
                    const editor = Object.values(CKEDITOR.instances)[0];
                    return editor && editor.status === 'ready';
                }
            ");

            string innerHtml = await contentFrame.EvaluateAsync<string>(@"
                () => Object.values(CKEDITOR.instances)[0]
                      .editable()
                      .getText()
                      .trim()
            ");

            await this.Page.WaitForAppIdleAsync();

            string textOnly = Regex.Replace(innerHtml, "<.*?>", string.Empty).Trim();

            return string.IsNullOrEmpty(textOnly) ? null : WebUtility.HtmlDecode(innerHtml);
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.AppPage.Page.WaitForAppIdleAsync();

            if (value != null)
            {
                await this.Container.WaitForAsync();
                await this.Container.ClickAndWaitForAppIdleAsync();

                var frame = await this.Container
                .Locator("iframe").Last.ElementHandleAsync();

                var contentFrame = await frame.ContentFrameAsync();

                await contentFrame.EvaluateAsync(
                @"(html) => {
                    const editor = Object.values(CKEDITOR.instances)[0];
                    editor.setData(html);
                    editor.fire('change');
                }",
                value);
            }
        }
    }
}