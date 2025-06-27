namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface representing a data set.
    /// </summary>
    [PlatformControl]
    public interface IDataSet : IControl
    {
        /// <summary>
        /// Gets the control within the dataset.
        /// </summary>
        /// <typeparam name="TPcfControl">The type of PCF control.</typeparam>
        /// <returns>The PCF control.</returns>
        TPcfControl GetControl<TPcfControl>()
            where TPcfControl : IPcfControl;

        /// <summary>
        /// Gets whether the data set is visible.
        /// </summary>
        /// <returns>A boolean indicating whether the data set is visible.</returns>
        Task<bool> IsVisibleAsync();

        /// <summary>
        /// Switches the view shown in the data set.
        /// </summary>
        /// <param name="viewName">The view name.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="PowerPlaywrightException">Thrown if the view selector is not available.</exception>"
        Task SwitchViewAsync(string viewName);

        /// <summary>
        /// Gets the active view in the data set (if the view selector is available).
        /// </summary>
        /// <returns>The view display name.</returns>
        /// <exception cref="PowerPlaywrightException">Thrown if the view selector is not available.</exception>"
        Task<string> GetActiveViewAsync();
    }
}
