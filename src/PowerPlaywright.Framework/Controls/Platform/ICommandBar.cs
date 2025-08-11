namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A command bar.
    /// </summary>
    [PlatformControl]
    public interface ICommandBar : IControl
    {
        /// <summary>
        /// Clicks a command.
        /// </summary>
        /// <param name="commands">The commands to click (in sequence).</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ClickCommandAsync(params string[] commands);

        /// <summary>
        /// Clicks a command that navigates to another page.
        /// </summary>
        /// <param name="commands">The commands to click (in sequence).</param>
        /// <typeparam name="TModelDrivenAppPage">The type of page.</typeparam>
        /// <returns>The page.</returns>
        Task<TModelDrivenAppPage> ClickCommandAsync<TModelDrivenAppPage>(params string[] commands)
            where TModelDrivenAppPage : IModelDrivenAppPage;

        /// <summary>
        /// Clicks a command that opens a dialog on the page.
        /// </summary>
        /// <param name="commands">The commands to click (in sequence).</param>
        /// <typeparam name="TControl">The type of dialog.</typeparam>
        /// <returns>The page.</returns>
        Task<TControl> ClickCommandWithDialogAsync<TControl>(params string[] commands)
            where TControl : IControl;

        /// <summary>
        /// Gets the commands visible on the command bar.
        /// </summary>
        /// <param name="parentCommands">An optional sequence of commands to use when checking getting nested commands.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<IEnumerable<string>> GetCommandsAsync(params string[] parentCommands);
    }
}
