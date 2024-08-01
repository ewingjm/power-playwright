namespace PowerPlaywright.Model.Controls
{
    using System.Reflection;

    /// <summary>
    /// Provides a mechanism for retrieving an assembly containing control strategies.
    /// </summary>
    public interface IControlStrategyAssemblyProvider
    {
        /// <summary>
        /// Get the control strategy assembly.
        /// </summary>
        /// <returns>The control strategy assembly.</returns>
        Assembly GetAssembly();
    }
}