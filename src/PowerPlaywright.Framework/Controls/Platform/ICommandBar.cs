using PowerPlaywright.Framework.Controls.Platform.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerPlaywright.Framework.Controls.Platform
{
    /// <summary>
    /// Implementation of the CommandBar control.
    /// </summary>
    [PlatformControl]
    public interface ICommandBar : IControl
    {
        /// <summary>
        /// Gets the commands visible on the command bar.
        /// </summary>
        /// <param name="includeOverFlowCommands">An optional flag to list the overflow commands in the form.</param>
        /// <param name="parentCommands">An optional sequence of commands to use when checking getting nested commands.</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetCommandsAsync(bool includeOverFlowCommands = true, params string[] parentCommands);

        /// <summary>
        /// Selects the commands on the command bar.
        /// </summary>
        /// <param name="commandName">The command to execute.</param>
        /// <returns></returns>
        Task SelectCommand(string commandName);
    }
}