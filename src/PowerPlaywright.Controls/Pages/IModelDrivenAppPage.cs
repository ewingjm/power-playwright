namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Model.Controls.Platform;

    /// <summary>
    /// A model-driven app page.
    /// </summary>
    public interface IModelDrivenAppPage
    {
        /// <summary>
        /// Gets the <see cref="IPage"/> for the model-driven app page.
        /// </summary>
        IPage Page { get; }

        /// <summary>
        /// Gets the site map control.
        /// </summary>
        ISiteMapControl SiteMap { get; }
    }
}