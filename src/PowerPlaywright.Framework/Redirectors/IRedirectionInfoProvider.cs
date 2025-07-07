namespace PowerPlaywright.Framework.Redirectors
{
    /// <summary>
    /// Provides control redirection info.
    /// </summary>
    public interface IRedirectionInfoProvider
    {
        /// <summary>
        /// Gets the redirection info.
        /// </summary>
        /// <returns>The redirection info.</returns>
        IRedirectionEnvironmentInfo GetRedirectionInfo();
    }
}