namespace PowerPlaywright.Model.Controls.Pcf.Attributes
{
    using System;

    /// <summary>
    /// Denotes a class as a strategy for a PCF control (<see cref="PcfControlAttribute"/>"/>).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PcfControlStrategyAttribute : Attribute
    {
        private Version version;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfControlStrategyAttribute"/> class.
        /// </summary>
        /// <param name="major">The major component of the control version this strategy applies from.</param>
        /// <param name="minor">The minor component of the control version this strategy applies from.</param>
        /// <param name="patch">The patch component of the control version this strategy applies from.</param>
        public PcfControlStrategyAttribute(uint major, uint minor, uint patch)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
        }

        /// <summary>
        /// Gets the major component of the control version this strategy applies from.
        /// </summary>
        public uint Major { get; }

        /// <summary>
        /// Gets the minor component of the control version this strategy applies from.
        /// </summary>
        public uint Minor { get; }

        /// <summary>
        /// Gets the patch component of the control version this strategy applies from.
        /// </summary>
        public uint Patch { get; }

        /// <summary>
        /// Gets the version this strategy applies from.
        /// </summary>
        public Version Version
        {
            get
            {
                if (this.version is null)
                {
                    this.version = new Version((int)this.Major, (int)this.Minor, (int)this.Patch);
                }

                return this.version;
            }
        }
    }
}
