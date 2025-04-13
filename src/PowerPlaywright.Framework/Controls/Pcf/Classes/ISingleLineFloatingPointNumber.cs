﻿namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Single-line text (Floating Point Number) control class.
    /// </summary>
    public interface ISingleLineFloatingPointNumber : IPcfControl
    {
        /// <summary>
        /// Sets the value of the single-line text (Floating Point Number) control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(double value);

        /// <summary>
        /// Gets the value of the single-line text (Floating Point Number) control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<double?> GetValueAsync();
    }
}