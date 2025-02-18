using PowerPlaywright.Framework.Controls.Pcf.Attributes;
using PowerPlaywright.Framework.Controls.Pcf;
using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Framework.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;

namespace PowerPlaywright.Strategies.Controls.Pcf
{
    [PcfControlStrategy(0, 0, 0)]
    public class UrlContol : PcfControl, IUrlControl
    {
        private readonly ILocator urlInput;

        public UrlContol(string name, IAppPage appPage, IControl parent = null) : base(name, appPage, parent)
        {
            this.urlInput = this.Container.Locator("input");
        }

        public async Task<string> GetValueAsync()
        {
            return await this.urlInput.InputValueAsync();
        }

        public async Task SetValueAsync(string value)
        {
            await this.urlInput.FillAsync(value);
        }

        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.FieldControls.UrlControl|{this.Name}.fieldControl|']");
        }
    }
}