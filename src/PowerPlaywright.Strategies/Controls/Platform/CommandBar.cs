namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A command bar.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class CommandBar : Control, ICommandBar
    {
        private readonly IPageFactory pageFactory;
        private readonly IControlFactory controlFactory;

        private readonly ILocator commands;
        private readonly ILocator overflowCommand;
        private readonly ILocator flyout;
        private readonly ILocator flyoutCommands;
        private readonly ILocator flyoutLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBar"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public CommandBar(IAppPage appPage, IPageFactory pageFactory, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            this.pageFactory = pageFactory;
            this.controlFactory = controlFactory;
            this.commands = this.Container.Locator("[role='menuitem']:not([data-id='OverflowButton']):not([aria-hidden='true'])");
            this.overflowCommand = this.Container.Locator("[data-id='OverflowButton']");
            this.flyout = this.Page.GetByRole(AriaRole.Menu);
            this.flyoutCommands = this.flyout.Locator("[role='menuitem']:not([id*='flyoutbackbutton']):not([aria-hidden='true'])");
            this.flyoutLoading = this.flyoutCommands.Filter(new LocatorFilterOptions { HasText = "Loading..." });
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetCommandsAsync(params string[] parentCommands)
        {
            await this.Page.WaitForAppIdleAsync();

            return parentCommands.Length == 0
                ? await this.GetRootCommandsAsync()
                : await this.GetNestedCommandsAsync(parentCommands);
        }

        /// <inheritdoc/>
        public async Task<TModelDrivenAppPage> ClickCommandAsync<TModelDrivenAppPage>(params string[] commands)
            where TModelDrivenAppPage : IModelDrivenAppPage
        {
            await this.ClickCommandAsync(commands);

            return await this.pageFactory.CreateInstanceAsync<TModelDrivenAppPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task ClickCommandAsync(params string[] commands)
        {
            await this.Page.WaitForAppIdleAsync();

            var parentCommands = commands.Take(commands.Length - 1);

            foreach (var parentCommand in parentCommands)
            {
                await this.ExpandCommandAsync(parentCommand);
            }

            var command = this.commands.Or(this.flyoutCommands).Filter(new LocatorFilterOptions { Has = this.Page.GetByText(commands.Last(), new PageGetByTextOptions { Exact = true }) });
            var commandFound = await command.IsVisibleAsync();

            if (!commandFound && commands.Length == 1 && await this.IsOverflowPresentAsync())
            {
                await this.ToggleOverflowAsync();
                commandFound = await command.IsVisibleAsync();
            }

            if (!commandFound)
            {
                throw new PowerPlaywrightException($"The command '{commands.Last()}' is not available in the command bar.");
            }

            if (await this.IsSplitButtonCommandAsync(command))
            {
                command = this.GetSplitButtonMainCommand(command);
            }

            await command.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<TControl> ClickCommandWithDialogAsync<TControl>(params string[] commands)
            where TControl : IControl
        {
            await this.ClickCommandAsync(commands);

            return this.controlFactory.CreateCachedInstance<TControl>(this.AppPage);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.GetByRole(AriaRole.Menubar, new LocatorGetByRoleOptions { Name = "Commands" }).First;
        }

        private async Task ExpandCommandAsync(string command)
        {
            await this.Page.WaitForAppIdleAsync();

            var initialCommand = this.commands.Or(this.flyoutCommands).Filter(new LocatorFilterOptions { HasText = command });

            var isRootCommand = await initialCommand.IsVisibleAsync();
            if (!isRootCommand && await this.IsOverflowPresentAsync())
            {
                await this.ToggleOverflowAsync();

                if (!await initialCommand.IsVisibleAsync())
                {
                    throw new PowerPlaywrightException($"The command '{initialCommand}' is not available in the command bar.");
                }
            }

            if (await this.IsSplitButtonCommandAsync(initialCommand))
            {
                initialCommand = this.GetSplitButtonDropdownCommand(initialCommand);
            }

            await initialCommand.ClickAndWaitForAppIdleAsync();

            await this.flyoutLoading.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden });
        }

        private async Task<IEnumerable<string>> GetRootCommandsAsync()
        {
            var labels = new List<string>();

            var commands = await this.commands.AllAsync();
            labels.AddRange(await Task.WhenAll(commands.Select(this.GetCommandLabel)));

            if (await this.IsOverflowPresentAsync())
            {
                await this.ToggleOverflowAsync();
            }

            var overflowCommands = await this.flyoutCommands.AllAsync();
            labels.AddRange(await Task.WhenAll(overflowCommands.Select(this.GetCommandLabel)));

            await this.Page.Keyboard.PressAsync("Escape");

            return labels;
        }

        private async Task<IEnumerable<string>> GetNestedCommandsAsync(string[] parentCommands)
        {
            foreach (var parentCommand in parentCommands)
            {
                await this.ExpandCommandAsync(parentCommand);
            }

            var nestedCommands = await this.flyoutCommands.AllAsync();

            var commands = await Task.WhenAll(nestedCommands.Select(this.GetCommandLabel));

            await this.Page.Keyboard.PressAsync("Escape");

            return commands;
        }

        private async Task<bool> IsOverflowPresentAsync()
        {
            return await this.overflowCommand.IsVisibleAsync();
        }

        private async Task ToggleOverflowAsync()
        {
            await this.overflowCommand.ClickAndWaitForAppIdleAsync();
        }

        private async Task<string> GetCommandLabel(ILocator command)
        {
            var label = await this.IsSplitButtonCommandAsync(command)
                ? await this.GetSplitButtonMainCommand(command).InnerTextAsync()
                : await command.InnerTextAsync();

            return label.Trim();
        }

        private ILocator GetSplitButtonMainCommand(ILocator command)
        {
            return command.Locator("[role='button']:not[aria-haspopup='true']");
        }

        private ILocator GetSplitButtonDropdownCommand(ILocator command)
        {
            return command.Locator("[role='button'][aria-haspopup='true']");
        }

        private async Task<bool> IsSplitButtonCommandAsync(ILocator command)
        {
            var id = await command.GetAttributeAsync(Attributes.Id);

            return id != null && id.Contains(".Menu0_splitButton");
        }
    }
}
