namespace PowerPlaywright
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Retrieves a control assembly from the test assembly directory.
    /// </summary>
    internal class LocalAssemblyProvider : IAssemblyProvider
    {
        private readonly string path;

        private Assembly assembly;

        public LocalAssemblyProvider(string path)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
        }

        /// <inheritdoc />
        public Assembly GetAssembly()
        {
            if (this.assembly == null)
            {
                var assemblyPath = Path.Combine(Path.GetDirectoryName(typeof(LocalAssemblyProvider).Assembly.Location), this.path);

                this.assembly = Assembly.LoadFrom(assemblyPath);
            }

            return this.assembly;
        }
    }
}
