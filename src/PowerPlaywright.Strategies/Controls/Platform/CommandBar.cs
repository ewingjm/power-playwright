using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PowerPlaywright.Framework.Controls;
using PowerPlaywright.Framework.Controls.Platform;
using PowerPlaywright.Framework.Controls.Platform.Attributes;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Strategies.Extensions;

namespace PowerPlaywright.Strategies.Controls.Platform
{
    /// <summary>
    /// CommandBar strategy.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class CommandBar : Control, ICommandBar
    {
        private readonly ILocator commands;
        private readonly ILocator overflowButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBar"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public CommandBar(IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
            commands = Container.GetByRole(AriaRole.Menuitem).Locator("span");
            overflowButton = Container.Locator("button[data-id='OverflowButton']");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetCommandsAsync(bool includeOverFlowCommands = true, params string[] parentCommands)
        {
            var visibleCommands = await GetCommandLabels(commands);

            if (!includeOverFlowCommands)
            {
                return visibleCommands;
            }

            await overflowButton.ClickAndWaitForAppIdleAsync();

            var flyoutId = await overflowButton.GetAttributeAsync("aria-owns");
            if (string.IsNullOrWhiteSpace(flyoutId))
            {
                return visibleCommands;
            }

            var flyout = Page.Locator($"#{flyoutId}");
            var overflowLocators = flyout.GetByRole(AriaRole.Menuitem).Locator("span");
            var overflowCommands = await GetCommandLabels(overflowLocators);

            return overflowCommands.Concat(visibleCommands).Distinct();
        }

        /// <inheritdoc/>
        public Task SelectCommand(string commandName)
        {
            throw new NotImplementedException();
        }

        private static async Task<IEnumerable<string>> GetCommandLabels(ILocator locator)
        {
            var elements = await locator.AllAsync();
            var texts = await Task.WhenAll(elements.Select(e => e.InnerTextAsync()));
            return texts.Where(text => !string.IsNullOrWhiteSpace(text));
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("ul[data-lp-id*=\"commandbar-Form:\"]");
        }
    }
}