namespace PowerPlaywright.Model.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Model.Controls.Pcf;
    using PowerPlaywright.Model.Controls.Platform.Attributes;

    /// <summary>
    /// An interface representing a form.
    /// </summary>
    [PlatformControl]
    public interface IMainFormControl : IControl
    {
        /// <summary>
        /// Opens a tab on the form.
        /// </summary>
        /// <param name="name">The tab name.</param>
        /// <returns>The control.</returns>
        Task OpenTabAsync(string name);

        /// <summary>
        /// Gets a control on the form.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="name">The control name.</param>
        /// <returns>The control.</returns>
        TControl GetControl<TControl>(string name)
            where TControl : IPcfControl;
    }
}
