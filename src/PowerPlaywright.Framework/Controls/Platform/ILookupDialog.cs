namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// A lookup dialog.
    /// </summary>
    [PlatformControl]
    public interface ILookupDialog : IPlatformControl
    {
        /// <summary>
        /// Gets the lookup.
        /// </summary>
        ILookup Lookup { get; }

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
        /// Saves the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveAsync();
    }
}
