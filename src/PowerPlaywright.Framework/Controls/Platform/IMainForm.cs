namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// An interface representing a form.
    /// </summary>
    [PlatformControl]
    public interface IMainForm : IPlatformControl
    {
        /// <summary>
        /// Gets the command bar on the form.
        /// </summary>
        ICommandBar CommandBar { get; }

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
        /// Gets all fields on the form (excluding header fields).
        /// </summary>
        /// <returns>The fields.</returns>
        Task<IEnumerable<IField>> GetFieldsAsync();

        /// <summary>
        /// Gets a field on the form with a known child control type.
        /// </summary>
        /// <typeparam name="TControl">The child control type.</typeparam>
        /// <param name="name">The field name.</param>
        /// <returns>The field.</returns>
        IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl;

        /// <summary>
        /// Gets a field on the form.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>The field.</returns>
        IField GetField(string name);

        /// <summary>
        /// Gets a quick view on the form.
        /// </summary>
        /// <param name="name">The quick view name.</param>
        /// <returns>The quick view.</returns>
        IQuickView GetQuickView(string name);

        /// <summary>
        /// Gets a data set on the form (e.g. a grid) with a know child control type.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="name">The name of the data set.</param>
        /// <returns>The data set.</returns>
        IDataSet<TControl> GetDataSet<TControl>(string name)
            where TControl : IPcfControl;

        /// <summary>
        /// Gets a data set on the form (e.g. a grid).
        /// </summary>
        /// <param name="name">The name of the data set.</param>
        /// <returns>The data set.</returns>
        IDataSet GetDataSet(string name);

        /// <summary>
        /// Expands the header flyout on the main form to enable interaction with header fields.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IMainFormHeader> ExpandHeaderAsync();

        /// <summary>
        /// Collapses the header flyout on the main form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CollapseHeaderAsync();
    }
}