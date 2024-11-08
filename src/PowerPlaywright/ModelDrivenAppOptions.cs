namespace PowerPlaywright
{
    using System;

    /// <summary>
    /// The options for the <see cref="ModelDrivenApp"/> class.
    /// </summary>
    internal class ModelDrivenAppOptions
    {
        /// <summary>
        /// Gets or sets the URL of the environment.
        /// </summary>
        public Uri EnvironmentUrl { get; set; }

        /// <summary>
        /// Gets or sets the app unique name.
        /// </summary>
        public string AppUniqueName { get; set; }
    }
}