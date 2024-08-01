namespace PowerPlaywright.Model.Controls.External
{
    using System;

    /// <summary>
    /// Denotes an interface as describing the interactions available for an external control.
    /// </summary>
    /// <remarks>
    /// An external control is any control that is not part of the Power Apps UI (for example, the login control).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class ExternalControlAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalControlAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the custom control (e.g. MscrmControls.Grid.ReadOnlyGrid).</param>
        public ExternalControlAttribute()
        {
        }
    }
}
