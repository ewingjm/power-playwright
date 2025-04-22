namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// A form field with a known PCF control type.
    /// </summary>
    /// <typeparam name="TPcfControl">The type of the field's control.</typeparam>
    [PlatformControl]
    public interface IFormField<out TPcfControl> : IFormField
        where TPcfControl : IPcfControl
    {
        /// <summary>
        /// Gets the PCF control.
        /// </summary>
        TPcfControl Control { get; }
    }
}
