namespace PowerPlaywright.Framework.Controls.Platform.Attributes
{
    using System;

    /// <summary>
    /// Denotes a class as a strategy for a platform control (<see cref="PlatformControlAttribute"/>"/>).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PlatformControlStrategyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformControlStrategyAttribute"/> class.
        /// </summary>
        /// <param name="major">The major component of the environment version that introduced this control strategy.</param>
        /// <param name="minor">The minor component of the environment version that introduced this control strategy.</param>
        /// <param name="build">The build component of the environment version that introduced this control strategy.</param>
        /// <param name="revision">The revision component of the environment version that introduced this control strategy.</param>
        public PlatformControlStrategyAttribute(uint major, uint minor, uint build, uint revision)
        {
            this.Version = new Version((int)major, (int)minor, (int)build, (int)revision);
        }

        /// <summary>
        /// Gets the environment version that this control strategy is applied from.
        /// </summary>
        public Version Version { get; }
    }
}