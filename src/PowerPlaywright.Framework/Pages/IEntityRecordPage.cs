namespace PowerPlaywright.Framework.Pages
{
    using PowerPlaywright.Framework.Controls.Platform;

    /// <summary>
    /// An entity form page.
    /// </summary>
    public interface IEntityRecordPage : IModelDrivenAppPage<IEntityRecordPageContent>
    {
        /// <summary>
        /// Gets the form.
        /// </summary>
        IMainForm Form { get; }
    }
}