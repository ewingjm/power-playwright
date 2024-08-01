namespace PowerPlaywright.Model.Controls.Platform
{
    using System;

    /// <summary>
    /// Denotes an interface as describing the interactions available for a platform control.
    /// </summary>
    /// <remarks>
    /// A platform control is control that appears within the Power Apps UI that is not a custom control.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class PlatformControlAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformControlAttribute"/> class.
        /// </summary>
        public PlatformControlAttribute()
        {
        }
    }
}
