namespace PowerPlaywright.Framework.Redirectors
{
    /// <summary>
    /// Provides control redirection info.
    /// </summary>
    /// <typeparam name="TControlRedirectorInfo">The type of control redirection info.</typeparam>
    public interface IRedirectionInfoProvider<out TControlRedirectorInfo>
    {
        /// <summary>
        /// Gets the redirection info.
        /// </summary>
        /// <returns>The redirection info.</returns>
        TControlRedirectorInfo GetRedirectionInfo();
    }
}
