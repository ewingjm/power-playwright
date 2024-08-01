namespace PowerPlaywright.Controls
{
    using System.IO;
    using System.Reflection;
    using PowerPlaywright.Model.Controls;

    /// <summary>
    /// Resolves a control assembly locally on disk.
    /// </summary>
    internal class LocalControlAssemblyProviders : IControlStrategyAssemblyProvider
    {
        private const string AssemblyName = "PowerPlaywright.Strategies.dll";

        /// <inheritdoc/>
        public Assembly GetAssembly()
        {
            return Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), AssemblyName));
        }
    }
}
