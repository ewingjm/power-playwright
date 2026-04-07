namespace PowerPlaywright.Framework.Pages
{
    using PowerPlaywright.Framework.Controls.Platform;

    /// <summary>
    /// An entity list page.
    /// </summary>
    public interface IEntityListPage : IModelDrivenAppPage<IEntityListPageContent>
    {
        /// <summary>
        /// Gets the grid.
        /// </summary>
        IDataSet DataSet { get; }
    }
}