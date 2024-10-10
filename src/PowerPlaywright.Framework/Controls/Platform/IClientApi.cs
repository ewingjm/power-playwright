namespace PowerPlaywright.Framework.Controls.Platform
{
    using System;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A control that is used as a helper to execute client API scripts.
    /// </summary>
    [PlatformControl]
    public interface IClientApi : IControl
    {
        /// <summary>
        /// Navigates to a record with the given logical name and ID.
        /// </summary>
        /// <param name="entityName">The table logical name.</param>
        /// <param name="entityId">The row ID.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEntityRecordPage> NavigateToRecordAsync(string entityName, Guid entityId);
    }
}