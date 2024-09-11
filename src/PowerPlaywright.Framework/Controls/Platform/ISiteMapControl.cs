namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An interface representing a sitemap.
    /// </summary>
    [PlatformControl]
    public interface ISiteMapControl : IControl
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
    }
}
