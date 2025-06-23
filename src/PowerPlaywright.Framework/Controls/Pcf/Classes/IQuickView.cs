namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    /// <summary>
    /// Quick view control class.
    /// </summary>
    public interface IQuickView : IPcfControl
    {
        /// <summary>
        /// Gets a field on the quick view.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <returns>The control.</returns>
        IField GetField(string name);
    }
}