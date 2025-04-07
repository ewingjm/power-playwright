namespace PowerPlaywright.Framework.Redirectors
{
    using System.Collections.Generic;

    /// <summary>
    /// The active user settings.
    /// </summary>
    public interface IUserSettings
    {
        /// <summary>
        /// Gets the trytogglesets user setting.
        /// </summary>
        IDictionary<string, bool?> AppToggles { get; }

        /// <summary>
        /// Gets the release channel user setting.
        /// </summary>
        int ReleaseChannel { get; }
    }
}
