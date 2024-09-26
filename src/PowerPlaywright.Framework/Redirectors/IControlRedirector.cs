namespace PowerPlaywright.Framework.Redirectors
{
    using System;
    using PowerPlaywright.Framework.Controls;

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
        /// <returns>The redirected type.</returns>
        Type Redirect();
    }
}
