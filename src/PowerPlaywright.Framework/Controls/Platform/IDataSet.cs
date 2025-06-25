namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// An interface representing a data set.
    /// </summary>
    [PlatformControl]
    public interface IDataSet : IControl
    {
        /// <summary>
        /// Gets the control within the dataset.
        /// </summary>
        /// <typeparam name="TPcfControl">The type of PCF control.</typeparam>
        /// <returns>The PCF control.</returns>
        TPcfControl GetControl<TPcfControl>()
            where TPcfControl : IPcfControl;
    }
}
