namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// An interface representing a data set with a known control type.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    [PlatformControl]
    public interface IDataSet<TControl> : IDataSet
        where TControl : IPcfControl
    {
        /// <summary>
        /// Gets the control within the dataset.
        /// </summary>
        TControl Control { get; }
    }
}
