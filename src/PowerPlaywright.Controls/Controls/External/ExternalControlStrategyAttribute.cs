namespace PowerPlaywright.Model.Controls.External
{
    using System;

    /// <summary>
    /// Denotes a class as a strategy for an external control (<see cref="ExternalControlAttribute"/>"/>).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ExternalControlStrategyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalControlStrategyAttribute"/> class.
        /// </summary>
        /// <param name="version">The version.</param>
        public ExternalControlStrategyAttribute(uint version)
        {
            this.Version = version;
        }

        /// <summary>
        /// Gets the version of the strategy.
        /// </summary>
        public uint Version { get; }
    }
}