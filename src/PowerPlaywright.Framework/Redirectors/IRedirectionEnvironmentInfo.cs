namespace PowerPlaywright.Framework.Redirectors
{
    /// <summary>
    /// Contextual environment information used to assist with control redirection (e.g. PCF classes to specific PCF controls).
    /// </summary>
    public interface IRedirectionEnvironmentInfo
    {
        /// <summary>
        /// Gets the org settings.
        /// </summary>
        IOrgSettings Org { get; }

        /// <summary>
        /// Gets the app settings.
        /// </summary>
        IAppSettings App { get; }

        /// <summary>
        /// Gets the user settings.
        /// </summary>
        IUserSettings User { get; }

        /// <summary>
        /// Gets the active release channel based on the user, app, and org settings.
        /// </summary>
        int ActiveReleaseChannel { get; }

        /// <summary>
        /// Gets a value indicating whether new look is enabled based on user, app, and org settings.
        /// </summary>
        bool IsNewLookEnabled { get; }

        /// <summary>
        /// Gets whether the relevance search is enabled on the instance.
        /// </summary>
        bool IsRelevanceSearchEnabled { get; }
    }
}