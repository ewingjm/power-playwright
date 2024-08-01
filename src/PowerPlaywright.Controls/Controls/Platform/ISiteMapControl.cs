namespace PowerPlaywright.Model.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Model.Controls;

    /// <summary>
    /// An interface representing a sitemap.
    /// </summary>
    [PlatformControl]
    public interface ISiteMapControl : IControl
    {
        /// <summary>
        /// Opens a page.
        /// </summary>
        /// <param name="area">The name of the area.</param>
        /// <param name="page">The name of the page.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task OpenPageAsync(string area, string page);
    }
}
