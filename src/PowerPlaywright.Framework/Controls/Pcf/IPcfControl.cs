namespace PowerPlaywright.Framework.Controls.Pcf
{
    /// <summary>
    /// A PCF control.
    /// </summary>
    public interface IPcfControl : IControl
    {
        /// <summary>
        /// Gets the name of the PCF control.
        /// </summary>
        string Name { get; }
    }
}