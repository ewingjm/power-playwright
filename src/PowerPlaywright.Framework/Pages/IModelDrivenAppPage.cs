namespace PowerPlaywright.Framework.Pages
{
    using System;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform;

    /// <summary>
    /// A model-driven app page.
    /// </summary>
    public interface IModelDrivenAppPage : IBasePage
    {
        /// <summary>
        /// Gets the site map control.
        /// </summary>
        ISiteMapControl SiteMap { get; }

        /// <summary>
        /// Navigates directly to a record.
        /// </summary>
        /// <param name="entityName">The logical name.</param>
        /// <param name="entityId">The id.</param>
        /// <returns>The entity record page.</returns>
        Task<IEntityRecordPage> NavigateToRecordAsync(string entityName, Guid entityId);

        /// <summary>
        /// Waits for the app to be idle.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task WaitForAppIdleAsync(TimeSpan timeout = default);
    }
}