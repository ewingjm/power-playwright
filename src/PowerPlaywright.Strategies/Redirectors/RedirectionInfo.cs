namespace PowerPlaywright.Strategies.Redirectors
{
    /// <summary>
    /// Runtime information used for control redirection.
    /// </summary>
    public class RedirectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectionInfo"/> class.
        /// </summary>
        /// <param name="orgSettings">The org settings.</param>
        /// <param name="appSettings">The app settings.</param>
        /// <param name="userSettings">The user settings.</param>
        internal RedirectionInfo(OrgSettings orgSettings, AppSettings appSettings, UserSettings userSettings)
        {
            this.OrgSettings = orgSettings;
            this.AppSettings = appSettings;
            this.UserSettings = userSettings;
        }

        /// <summary>
        /// Gets the org settings.
        /// </summary>
        internal OrgSettings OrgSettings { get; private set; }

        /// <summary>
        /// Gets the app settings.
        /// </summary>
        internal AppSettings AppSettings { get; private set; }

        /// <summary>
        /// Gets the user settings.
        /// </summary>
        internal UserSettings UserSettings { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the new look is enabled.
        /// </summary>
        internal bool IsNewLookEnabled
        {
            get
            {
                if ((bool)this.AppSettings.NewLookAlwaysOn)
                {
                    return true;
                }

                if ((bool)!this.AppSettings.NewLookOptOut)
                {
                    return false;
                }

                if (this.UserSettings.TryToggleSets != null && this.UserSettings.TryToggleSets.ModernizationOptOut.HasValue)
                {
                    return this.UserSettings.TryToggleSets.ModernizationOptOut.Value;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the active release channel.
        /// </summary>
        /// <returns>The active release channel.</returns>
        internal ReleaseChannel ActiveReleaseChannel
        {
            get
            {
                if (this.UserSettings.ReleaseChannel != ReleaseChannelOverride.None)
                {
                    switch (this.UserSettings.ReleaseChannel)
                    {
                        case ReleaseChannelOverride.SemiAnnual:
                            return ReleaseChannel.SemiAnnualChannel;

                        case ReleaseChannelOverride.Inner:
                            return ReleaseChannel.MicrosoftInnerChannel;

                        case ReleaseChannelOverride.Monthly:
                            return ReleaseChannel.Monthly;
                    }
                }

                if (this.AppSettings != null && this.AppSettings.AppChannel != ReleaseChannel.Auto)
                {
                    return (ReleaseChannel)this.AppSettings.AppChannel;
                }

                if (this.OrgSettings.ReleaseChannel != ReleaseChannel.Auto)
                {
                    return this.OrgSettings.ReleaseChannel;
                }

                return ReleaseChannel.Monthly;
            }
        }
    }
}