namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Pcf;

    /// <summary>
    /// An interface representing a data set.
    /// </summary>
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
