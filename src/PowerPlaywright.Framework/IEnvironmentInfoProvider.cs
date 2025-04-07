namespace PowerPlaywright.Framework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides information about the environment.
    /// </summary>
    public interface IEnvironmentInfoProvider
    {
        /// <summary>
        /// An event fired when the info provider is ready.
        /// </summary>
        event EventHandler OnReady;

        /// <summary>
        /// Gets a value indicating whether whether or not the environment info provider is ready.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Gets the platform version.
        /// </summary>
        Version PlatformVersion { get; }

        /// <summary>
        /// Gets the control versions.
        /// </summary>
        IDictionary<string, Version> ControlVersions { get; }

        /// <summary>
        /// Gets the control IDs.
        /// </summary>
        IDictionary<string, Guid> ControlIds { get; }
    }
}