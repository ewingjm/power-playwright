namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides information about the environment.
    /// </summary>
    internal interface IEnvironmentInfoProvider
    {
        /// <summary>
        /// Gets the platform version.
        /// </summary>
        Version PlatformVersion { get; }

        /// <summary>
        /// Gets the control versions.
        /// </summary>
        IDictionary<string, Version> ControlVersions { get; }
    }
}