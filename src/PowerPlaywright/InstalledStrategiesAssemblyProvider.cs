namespace PowerPlaywright
{
    using System.Reflection;
    using PowerPlaywright.Strategies.Redirectors;

    /// <summary>
    /// Provides the built-in strategies shipped with this release.
    /// </summary>
    internal class InstalledStrategiesAssemblyProvider : IAssemblyProvider
    {
        /// <inheritdoc/>
        public Assembly GetAssembly()
        {
            return typeof(RedirectionInfoProvider).Assembly;
        }
    }
}
