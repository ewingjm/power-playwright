namespace PowerPlaywright.Assemblies
{
    using System.Reflection;

    /// <summary>
    /// Provides a mechanism for retrieving an assembly containing control strategies.
    /// </summary>
    internal interface IControlStrategyAssemblyProvider
    {
        /// <summary>
        /// Get the control strategy assembly.
        /// </summary>
        /// <returns>The control strategy assembly.</returns>
        Assembly GetAssembly();
    }
}