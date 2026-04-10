namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// The contents of an entity list page content.
    /// </summary>
    [PlatformControl]
    public interface IEntityListPageContent : IModelDrivenAppPageContent
    {
        /// <summary>
        /// Gets the grid.
        /// </summary>
        IDataSet DataSet { get; }
    }
}