namespace PowerPlaywright.Framework.Redirectors
{
    /// <summary>
    /// Runtime information used for control redirection.
    /// </summary>
    public class RedirectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectionInfo"/> class.
        /// </summary>
        /// <param name="appToggles">The app toggles.</param>
        public RedirectionInfo(AppToggles appToggles)
        {
            this.AppToggles = appToggles;
        }

        /// <summary>
        /// Gets the user app toggles.
        /// </summary>
        public AppToggles AppToggles { get; private set; }
    }
}