namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// An interface representing a form.
    /// </summary>
    [PlatformControl]
    public interface IMainFormControl : IControl
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
        /// Gets a control on the form.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="name">The control name.</param>
        /// <returns>The control.</returns>
        TControl GetControl<TControl>(string name)
            where TControl : IPcfControl;
    }
}
