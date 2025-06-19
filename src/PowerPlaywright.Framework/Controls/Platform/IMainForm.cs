namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// An interface representing a form.
    /// </summary>
    [PlatformControl]
    public interface IMainForm : IPlatformControl
    {
        /// <summary>
        /// Opens a tab on the form.
        /// </summary>
        /// <param name="label">The tab label.</param>
        /// <returns>The control.</returns>
        Task OpenTabAsync(string label);

        /// <summary>
        /// Gets the active tab.
        /// </summary>
        /// <returns>The active tab label.</returns>
        Task<string> GetActiveTabAsync();

        /// <summary>
        /// Gets all visible tabs on the form.
        /// </summary>
        /// <returns>A collection of visible tabs on the form.</returns>
        Task<IEnumerable<string>> GetAllTabsAsync();

        /// <summary>
        /// Gets a value indicating whether the form is disabled.
        /// </summary>
        /// <returns>A value indicating whether the form is disabled.</returns>
        Task<bool> IsDisabledAsync();

        /// <summary>
        /// Gets a field on the form.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <returns>The control.</returns>
        IFormField GetField(string name);

        /// <summary>
        /// Gets a field on the form with a known PCF control or control class type.
        /// </summary>
        /// <typeparam name="TPcfControl">The PCF control type.</typeparam>
        /// <param name="name">The control name.</param>
        /// <returns>The control.</returns>
        IFormField<TPcfControl> GetField<TPcfControl>(string name)
            where TPcfControl : IPcfControl;
    }
}