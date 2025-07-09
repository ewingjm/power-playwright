namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Quick view control class.
    /// </summary>
    public interface IQuickView : IPcfControl
    {
        /// <summary>
        /// Gets a field on the quick view with a known child control type.
        /// </summary>
        /// <typeparam name="TControl">The control type.</typeparam>
        /// <param name="name">The control name.</param>
        /// <returns>The control.</returns>
        IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl;

        /// <summary>
        /// Gets a field on the quick view.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <returns>The control.</returns>
        IField GetField(string name);

        /// <summary>
        /// Gets whether the quick view is visible.
        /// </summary>
        /// <returns>A boolean indicating whether the quick view is visible.</returns>
        Task<bool> IsVisibleAsync();
    }
}