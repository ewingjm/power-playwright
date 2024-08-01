namespace PowerPlaywright.Model.Redirectors
{
    using System;
    using System.Collections.Generic;
    using PowerPlaywright.Model.Controls;

    /// <summary>
    /// A control redirector.
    /// </summary>
    /// <typeparam name="TSourceControl">The type of control redirected from.</typeparam>
    public abstract class ControlRedirector<TSourceControl> : IControlRedirector<TSourceControl>
        where TSourceControl : IControl
    {
        /// <inheritdoc/>
        public abstract Type Redirect(ControlRedirectionInfo redirectionInfo, IEnumerable<Type> controlTypes);
    }
}
