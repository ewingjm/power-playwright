namespace PowerPlaywright.Model.Redirectors
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Model.Controls;

    /// <summary>
    /// Redirects controls of one type to another (compatible) type.
    /// </summary>
    /// <typeparam name="TSourceControl">The type of control redirected.</typeparam>
    public interface IControlRedirector<out TSourceControl>
        where TSourceControl : IControl
    {
        /// <summary>
        /// Gets the control interface that controls of the provided type should be redirected to.
        /// </summary>
        /// <param name="redirectionInfo">The redirection info.</param>
        /// <param name="controlTypes">All control interfaces.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The redirected interface or null if not redirected.</returns>
        Type Redirect(ControlRedirectionInfo redirectionInfo, IEnumerable<Type> controlTypes);
    }
}
