namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// An assign dialog.
    /// </summary>
    [PlatformControl]
    public interface IAssignDialog : IPlatformControl
    {
        /// <summary>
        /// Gets whether the dialog is visible.
        /// </summary>
        /// <returns>A boolean indicating whether the dialog is visible.</returns>
        Task<bool> IsVisibleAsync();

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CancelAsync();

        /// <summary>
        /// Assigns the record to me.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AssignToMeAsync();

        /// <summary>
        /// Assigns the record to a user or team.
        /// </summary>
        /// <param name="userOrTeam">The user or team name.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AssignToUserOrTeamAsync(string userOrTeam);

        /// <summary>
        /// Gets the current assign to selection.
        /// </summary>
        /// <returns>The current assign to selection ("Me" or "User or team").</returns>
        Task<string> GetAssignToAsync();
    }
}
