namespace PowerPlaywright.Model.Controls.Pcf.Attributes
{
    using System;

    /// <summary>
    /// Denotes an interface as describing the interactions available for a specific PCF control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class PcfControlAttribute : Attribute
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfControlAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the PCF control (e.g. MscrmControls.Grid.ReadOnlyGrid).</param>
        public PcfControlAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the name of the custom control.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
    }
}
