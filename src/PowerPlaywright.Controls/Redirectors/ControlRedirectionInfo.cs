namespace PowerPlaywright.Model.Redirectors
{
    /// <summary>
    /// Runtime information used for control redirection.
    /// </summary>
    public class ControlRedirectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlRedirectionInfo"/> class.
        /// </summary>
        /// <param name="appToggles">The app toggles.</param>
        public ControlRedirectionInfo(AppToggles appToggles)
        {
            this.AppToggles = appToggles;
        }

        /// <summary>
        /// Gets the user app toggles.
        /// </summary>
        public AppToggles AppToggles { get; private set; }
    }
}
