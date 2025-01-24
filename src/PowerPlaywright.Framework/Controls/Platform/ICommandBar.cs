using PowerPlaywright.Framework.Controls.Platform.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerPlaywright.Framework.Controls.Platform
{
    [PlatformControl]
    public interface ICommandBar : IControl
    {
        /// <summary>
        /// Gets the commands visible on the command bar.
        /// </summary>
        /// <param name="parentCommands">An optional sequence of commands to use when checking getting nested commands.</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetCommandsAsync(params string[] parentCommands);
    }
}
