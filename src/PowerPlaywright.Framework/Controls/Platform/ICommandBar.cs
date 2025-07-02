namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A command bar.
    /// </summary>
    [PlatformControl]
    public interface ICommandBar : IControl
    {
        /// <summary>
        /// Clicks a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parentCommands">The parent commands (if any).</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ClickCommandAsync(string command, params string[] parentCommands);

        /// <summary>
        /// Gets the commands visible on the command bar.
        /// </summary>
        /// <param name="parentCommands">An optional sequence of commands to use when checking getting nested commands.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<IEnumerable<string>> GetCommandsAsync(params string[] parentCommands);
    }
}
