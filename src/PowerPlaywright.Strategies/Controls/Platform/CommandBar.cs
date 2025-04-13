using Microsoft.Playwright;
using PowerPlaywright.Framework.Controls;
using PowerPlaywright.Framework.Controls.Platform;
using PowerPlaywright.Framework.Controls.Platform.Attributes;
using PowerPlaywright.Framework.Pages;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace PowerPlaywright.Strategies.Controls.Platform
{
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class CommandBar : Control, ICommandBar
    {
        private readonly ILocator commands;

        public CommandBar(IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
            this.commands = Container.GetByRole(AriaRole.Listitem).GetByRole(AriaRole.Menuitem);
        }

        public async Task<IEnumerable<string>> GetCommandsAsync(params string[] parentCommands)
        {
            var commandLocators = await this.commands.AllAsync();

            return await Task.WhenAll(commandLocators.Select(c => c.GetAttributeAsync("aria-label")));
        }

        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("ul[data-lp-id*=\"commandbar-Form:\"]");
        }
    }
}
