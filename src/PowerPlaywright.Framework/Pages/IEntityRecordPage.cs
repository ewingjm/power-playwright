namespace PowerPlaywright.Framework.Pages
{
    using PowerPlaywright.Framework.Controls.Platform;

    /// <summary>
    /// An entity form page.
    /// </summary>
    public interface IEntityRecordPage : IModelDrivenAppPage
    {
        /// <summary>
        /// Gets the form.
        /// </summary>
        IMainFormControl Form { get; }

        /// <summary>
        /// Gets the command bar.
        /// </summary>
        ICommandBar CommandBar { get; }
    }
}