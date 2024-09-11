namespace PowerPlaywright.Framework.Pages
{
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// An entity list page.
    /// </summary>
    public interface IEntityListPage : IModelDrivenAppPage
    {
        /// <summary>
        /// Gets the grid.
        /// </summary>
        IReadOnlyGrid Grid { get; }
    }
}