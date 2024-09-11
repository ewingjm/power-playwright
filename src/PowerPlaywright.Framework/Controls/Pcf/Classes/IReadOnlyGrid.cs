namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Enums;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Read-only grid control class.
    /// </summary>
    [PcfControlClass("E7A81278-8635-4d9e-8D4D-59480B391C5B", DataType.DataSet, "Read-only grid")]
    public interface IReadOnlyGrid : IPcfControl
    {
        /// <summary>
        /// Opens the record at the provided zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the record.</param>
        /// <returns>The record.</returns>
        Task<IEntityRecordPage> OpenRecordAsync(int index);
    }
}
