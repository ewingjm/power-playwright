﻿namespace PowerPlaywright.Framework.Redirectors
{
    /// <summary>
    /// The active org settings.
    /// </summary>
    public interface IOrgSettings
    {
        /// <summary>
        /// Gets the release channel org setting.
        /// </summary>
        int ReleaseChannel { get; }
    }
}
