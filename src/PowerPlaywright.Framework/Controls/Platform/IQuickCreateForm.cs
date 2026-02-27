namespace PowerPlaywright.Framework.Controls.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Model;

    /// <summary>
    /// An interface representing a quick create form.
    /// </summary>
    [PlatformControl]
    public interface IQuickCreateForm : IPlatformControl
    {
        /// <summary>
        /// Gets all fields on the form.
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
        /// Gets the form notifications.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<FormNotification>> GetFormNotificationsAsync();

        /// <summary>
        /// Cancels the quick create.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CancelAsync();

        /// <summary>
        /// Save and close the quick create.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveAndCloseAsync();
    }
}