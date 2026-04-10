namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// The contents of an entity record page content.
    /// </summary>
    [PlatformControl]
    public interface IEntityRecordPageContent : IModelDrivenAppPageContent
    {
        /// <summary>
        /// Gets the form.
        /// </summary>
        IMainForm Form { get; }
    }
}