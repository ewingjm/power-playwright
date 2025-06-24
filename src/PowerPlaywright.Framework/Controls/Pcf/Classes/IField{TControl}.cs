namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    /// <summary>
    /// Typed field class.
    /// </summary>
    /// <typeparam name="TControl">The type of control.</typeparam>
    public interface IField<out TControl> : IField
        where TControl : IPcfControl
    {
        /// <summary>
        /// Gets the control.
        /// </summary>
        TControl Control { get; }
    }
}
