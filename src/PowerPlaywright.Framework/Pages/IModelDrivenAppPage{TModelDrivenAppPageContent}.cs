namespace PowerPlaywright.Framework.Pages
{
    using PowerPlaywright.Framework.Controls.Platform;

    /// <summary>
    /// Model-driven app page.
    /// </summary>
    /// <typeparam name="TModelDrivenAppPageContent">The type of content contained within the page.</typeparam>
    public interface IModelDrivenAppPage<out TModelDrivenAppPageContent> : IModelDrivenAppPage
        where TModelDrivenAppPageContent : IModelDrivenAppPageContent
    {
        /// <summary>
        /// Gets the page content.
        /// </summary>
        TModelDrivenAppPageContent Content { get; }
    }
}