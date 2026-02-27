namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An interface representing a sitemap.
    /// </summary>
    [PlatformControl]
    public interface ISiteMap : IPlatformControl
    {
        /// <summary>
        /// Opens a page.
        /// </summary>
        /// <typeparam name="TPage">The type of page to open.</typeparam>
        /// <param name="area">The name of the area.</param>
        /// <param name="group">The name of the group.</param>
        /// <param name="page">The name of the page.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TPage> OpenPageAsync<TPage>(string area, string group, string page)
            where TPage : IModelDrivenAppPage;

        /// <summary>
        /// Gets the areas.
        /// </summary>
        /// <returns>The areas.</returns>
        Task<IEnumerable<string>> GetAreasAsync();

        /// <summary>
        /// Gets the groups in a specified area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns>The groups.</returns>
        Task<IEnumerable<string>> GetGroupsAsync(string area);

        /// <summary>
        /// Gets the pages in a specified area and (optional) group.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="group">The group.</param>
        /// <returns>The pages.</returns>
        Task<IEnumerable<string>> GetPagesAsync(string area, string group = null);
    }
}