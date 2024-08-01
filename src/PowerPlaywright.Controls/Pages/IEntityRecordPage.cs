namespace PowerPlaywright.Pages
{
    using PowerPlaywright.Model.Controls.Platform;

    /// <summary>
    /// An entity form page.
    /// </summary>
    public interface IEntityRecordPage : IModelDrivenAppPage
    {
        /// <summary>
        /// Gets the form.
        /// </summary>
        IMainFormControl Form { get; }
    }
}
