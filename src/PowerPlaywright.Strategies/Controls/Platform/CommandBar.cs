using Microsoft.Playwright;
using PowerPlaywright.Framework.Controls;
using PowerPlaywright.Framework.Controls.Platform;
using PowerPlaywright.Framework.Controls.Platform.Attributes;
using PowerPlaywright.Framework.Pages;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using PowerPlaywright.Framework.Extensions;
using PowerPlaywright.Framework;

namespace PowerPlaywright.Strategies.Controls.Platform
{
    /// <summary>
    /// A command bar
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class CommandBar : Control, ICommandBar
    {
        private readonly ILocator commands;
        private readonly ILocator overflowButton;
        private readonly ILocator overflowFlyout;
        private readonly ILocator overflowCommands;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBar"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public CommandBar(IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
            this.commands = this.Container.Locator("button");
            this.overflowButton = this.Container.Locator("button[data-id='OverflowButton']");
            this.overflowFlyout = this.Page.Locator("ul[data-id='OverflowFlyout']");
            this.overflowCommands = this.overflowFlyout.Locator("button");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetCommandsAsync(params string[] parentCommands)
        {
            await this.Page.WaitForAppIdleAsync();

            if (parentCommands.Length != 0)
            {
                var commandFlyout = await this.ExpandCommandsAsync(parentCommands);

                return await this
                    .GetFlyoutCommandsLocator(commandFlyout)
                    .AllTextContentsAsync();
            }

            if (await IsOverflowPresentAsync())
            {
                await this.ExpandOverflowAsync();
            }

            var commands = await this.commands.AllTextContentsAsync();
            var overflowCommands = await this.overflowFlyout.AllTextContentsAsync();

            return commands.Concat(overflowCommands).Where(c => !string.IsNullOrEmpty(c)).ToList();
        }

        /// <inheritdoc/>
        public async Task ClickCommandAsync(string command, params string[] parentCommands)
        {
            var commandLocator = this.GetCommandLocator(command);

            if (parentCommands.Length != 0)
            {
                var flyout = await this.ExpandCommandsAsync(parentCommands);
                commandLocator = this.GetFlyoutCommandLocator(command, flyout);
            }
            else if (!await this.IsCommandVisibleAsync(command) && await this.IsOverflowPresentAsync())
            {
                await this.ExpandOverflowAsync();
                commandLocator = this.GetOverflowCommandLocator(command);
            }

            if (!await commandLocator.IsVisibleAsync())
            {
                throw new NotFoundException($"Unable to find command {string.Join("/", parentCommands)}{command}.");
            }

            await commandLocator.ClickAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("ul[data-lp-id*=\"commandbar-Form:\"]");
        }

        private async Task<ILocator> ExpandCommandsAsync(string[] commands)
        {
            if (!commands.Any())
            {
                throw new ArgumentException("At least one command must be specified.", nameof(commands));
            }

            var rootCommand = commands.First();
            var isOverflow = false;

            if (!await IsCommandVisibleAsync(rootCommand))
            {
                await ExpandOverflowAsync();

                if (!await IsCommandVisibleInOverflowAsync(rootCommand))
                {
                    throw new NotFoundException($"The root command '{rootCommand}' was not found in the command bar.");
                }

                isOverflow = true;
            }


            var flyout = await ExpandCommandAsync(rootCommand, isOverflow: isOverflow, isChild: false);

            foreach (var command in commands.Skip(1))
            {
                flyout = await ExpandCommandAsync(command, isOverflow: isOverflow, isChild: true);
            }

            return flyout;
        }

        private async Task<ILocator> ExpandCommandAsync(string command, bool isOverflow = false, bool isChild = false)
        {
            ILocator commandLocator;

            if (isOverflow)
            {
                commandLocator = this.GetOverflowCommandLocator(command);
            }
            else
            {
                commandLocator = this.GetCommandLocator(command);
            }

            if (!await commandLocator.IsVisibleAsync())
            {
                throw new NotFoundException($"The command '{command}' was not found in the command bar.");
            }

            var expandLocator = commandLocator;

            if (await commandLocator.First.GetAttributeAsync("aria-haspopup") == null)
            {
                // Split button
                expandLocator = commandLocator.First.Locator("..").Locator("button[aria-haspopup='true']");

                if (!await expandLocator.IsVisibleAsync())
                {
                    throw new NotSupportedException($"Failed to expand '{command}'. Command was visible but was not a dropdown or split button.");
                }
            }

            await expandLocator.ClickAsync();
            await this.Page.WaitForAppIdleAsync();

            var flyoutId = await expandLocator.GetAttributeAsync("aria-owns");

            return this.Page.Locator($"div[id='{flyoutId}'");
        }

        private async Task<bool> IsCommandVisibleInOverflowAsync(string command)
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.GetOverflowCommandLocator(command).IsVisibleAsync();
        }

        private async Task<bool> IsCommandVisibleAsync(string command)
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.GetCommandLocator(command).IsVisibleAsync();
        }

        private async Task<bool> IsOverflowPresentAsync()
        {
            return await this.overflowButton.IsVisibleAsync();
        }

        private ILocator GetFlyoutCommandLocator(string command, ILocator flyout)
        {
            return GetFlyoutCommandsLocator(flyout).Filter(new LocatorFilterOptions() { HasText = command });
        }

        private ILocator GetFlyoutCommandsLocator(ILocator flyout)
        {
            return flyout.Locator("button");
        }

        private ILocator GetOverflowCommandLocator(string command)
        {
            return this.overflowCommands.Filter(new LocatorFilterOptions() { HasText = command });
        }

        private ILocator GetCommandLocator(string command)
        {
            return this.commands.Filter(new LocatorFilterOptions { HasText = command });
        }

        private async Task ExpandOverflowAsync()
        {
            await this.overflowButton.ClickAsync();
            await this.Page.WaitForAppIdleAsync();
        }
    }
}
