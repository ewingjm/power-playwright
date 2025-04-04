namespace PowerPlaywright.Framework.Redirectors
{
    /// <summary>
    /// The active app settings.
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets the value of the NewLookOptOut app setting.
        /// </summary>
        bool NewLookOptOut { get; }

        /// <summary>
        /// Gets the value of the NewLookAlwaysOn app setting.
        /// </summary>
        bool NewLookAlwaysOn { get; }

        /// <summary>
        /// Gets the value of the AppChannel app setting.
        /// </summary>
        int AppChannel { get; }
    }
}
