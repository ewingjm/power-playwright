namespace PowerPlaywright.Framework.Pages
{
    using System;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform;

    /// <summary>
    /// A model-driven app page.
    /// </summary>
    public interface IModelDrivenAppPage : IAppPage
    {
        /// <summary>
        /// Gets the site map control.
        /// </summary>
        ISiteMapControl SiteMap { get; }

        /// <summary>
        /// Gets an object that provides client API functionality.
        /// </summary>
        IClientApi ClientApi { get; }
    }
}