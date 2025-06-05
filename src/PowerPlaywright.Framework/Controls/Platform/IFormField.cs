namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using System.Threading.Tasks;

    /// <summary>
    /// A form field.
    /// </summary>
    [PlatformControl]
    public interface IFormField : IControl
    {
        /// <summary>
        /// Gets the name for the control.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets whether the control is editable.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> IsDisabledAsync();

        /// <summary>
        /// Gets whether the control is mandatory.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> IsMandatoryAsync();

        /// <summary>
        /// Gets whether the control is editable.
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
