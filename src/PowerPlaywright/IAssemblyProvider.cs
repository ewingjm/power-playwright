namespace PowerPlaywright
{
    using System.Reflection;

    /// <summary>
    /// Provides an assembly.
    /// </summary>
    internal interface IAssemblyProvider
    {
        /// <summary>
        /// Get the control strategy assembly.
        /// </summary>
        /// <returns>The control strategy assembly.</returns>
        Assembly GetAssembly();
    }
}