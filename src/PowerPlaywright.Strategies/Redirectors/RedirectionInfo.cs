namespace PowerPlaywright.Strategies.Redirectors
{
    using PowerPlaywright.Framework.Redirectors;
    using System;

    /// <summary>
    /// Runtime information used for control redirection.
    /// </summary>
    public class RedirectionInfo : IRedirectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectionInfo"/> class.
        /// </summary>
        /// <param name="version">The environment version.</param>
        /// <param name="orgSettings">The org settings.</param>
        /// <param name="appSettings">The app settings.</param>
        /// <param name="userSettings">The user settings.</param>
        internal RedirectionInfo(Version version, OrgSettings orgSettings, AppSettings appSettings, UserSettings userSettings)
        {
            this.Version = version;
            this.OrgSettings = orgSettings;
            this.AppSettings = appSettings;
            this.UserSettings = userSettings;
        }

        /// <inheritdoc />
        public IOrgSettings Org => this.OrgSettings;

        /// <inheritdoc />
        public IAppSettings App => this.AppSettings;

        /// <inheritdoc />
        public IUserSettings User => this.UserSettings;

        /// <summary>
        /// Gets the environment version.k
        /// </summary>
        internal Version Version { get; }

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
        public bool IsNewLookEnabled
        {
            get
            {
                if (this.App.NewLookAlwaysOn)
                {
                    return true;
                }

                if (!this.App.NewLookOptOut)
                {
                    return false;
                }

                if (this.User.AppToggles != null && this.UserSettings.AppTogglesTyped.ModernizationOptOut.HasValue && this.Version < Version.Parse("9.2.25031.180") )
                {
                    return this.UserSettings.AppTogglesTyped.ModernizationOptOut.Value;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the active release channel.
        /// </summary>
        /// <returns>The active release channel.</returns>
        public int ActiveReleaseChannel
        {
            get
            {
                if (this.UserSettings.ReleaseChannelEnum != ReleaseChannelOverride.None)
                {
                    switch (this.UserSettings.ReleaseChannelEnum)
                    {
                        case ReleaseChannelOverride.SemiAnnual:
                            return (int)ReleaseChannel.SemiAnnualChannel;
                        case ReleaseChannelOverride.Inner:
                            return (int)ReleaseChannel.MicrosoftInnerChannel;
                        case ReleaseChannelOverride.Monthly:
                            return (int)ReleaseChannel.Monthly;
                    }
                }

                if (this.AppSettings.AppChannelEnum != ReleaseChannel.Auto)
                {
                    return (int)this.AppSettings.AppChannelEnum;
                }

                if (this.OrgSettings.ReleaseChannelEnum != ReleaseChannel.Auto)
                {
                    return (int)this.OrgSettings.ReleaseChannelEnum;
                }

                return (int)ReleaseChannel.Monthly;
            }
        }
    }
}