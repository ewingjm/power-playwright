namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Enums;

    /// <summary>
    /// Lookup control class.
    /// </summary>
    [PcfControlClass("270BD3DB-D9AF-4782-9025-509E298DEC0A", DataType.LookupSimple)]
    public interface ILookup : IPcfControl
    {
        /// <summary>
        /// Sets the value of the lookup.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);

        /// <summary>
        /// Gets the value of the lookup.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();
    }
}
