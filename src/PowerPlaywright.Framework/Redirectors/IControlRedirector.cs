namespace PowerPlaywright.Framework.Redirectors
{
    using System;
    using PowerPlaywright.Framework.Controls;

    /// <summary>
    /// Redirects controls of one type to another (compatible) type.
    /// </summary>
    /// <typeparam name="TRedirectionInfo">The type of redirection info.</typeparam>
    /// <typeparam name="TSourceControl">The type of control redirected.</typeparam>
    public interface IControlRedirector<in TRedirectionInfo, out TSourceControl>
        where TSourceControl : IControl
    {
        /// <summary>
        /// Gets the control interface that controls of the provided type should be redirected to.
        /// </summary>
        /// <param name="redirectionInfo">The redirection info.</param>
        /// <returns>The redirected type.</returns>
        Type Redirect(TRedirectionInfo redirectionInfo);
    }
}
