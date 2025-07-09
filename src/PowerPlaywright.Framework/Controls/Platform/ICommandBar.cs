namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

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
        /// Gets the commands visible on the command bar.
        /// </summary>
        /// <param name="parentCommands">An optional sequence of commands to use when checking getting nested commands.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<IEnumerable<string>> GetCommandsAsync(params string[] parentCommands);
    }
}
