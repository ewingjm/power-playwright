namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using PowerPlaywright.Framework.Model;
    using System.Threading.Tasks;

    /// <summary>
    /// Field class.
    /// </summary>
    public interface IField : IPcfControl
    {
        /// <summary>
        /// Gets a control within the field.
        /// </summary>
        /// <typeparam name="TPcfControl">The type of PCF control.</typeparam>
        /// <returns>The PCF control.</returns>
        TPcfControl GetControl<TPcfControl>()
            where TPcfControl : IPcfControl;

        /// <summary>
        /// Gets whether the field is editable.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> IsDisabledAsync();

        /// <summary>
        /// Gets the field requirement level.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<FieldRequirementLevel> GetRequirementLevelAsync();

        /// <summary>
        /// Gets whether the control is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> IsVisibleAsync();

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetLabelAsync();
    }
}
