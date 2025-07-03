namespace PowerPlaywright.Framework.Redirectors
{
    /// <summary>
    /// The active app settings.
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets a value indicating whether the NewLookOptOut app setting is enabled.
        /// </summary>
        bool NewLookOptOut { get; }

        /// <summary>
        /// Gets a value indicating whether the NewLookAlwaysOn app setting is enabled.
        /// </summary>
        bool NewLookAlwaysOn { get; }

        /// <summary>
        /// Gets the value of the AppChannel app setting.
        /// </summary>
        int AppChannel { get; }
    }
}
