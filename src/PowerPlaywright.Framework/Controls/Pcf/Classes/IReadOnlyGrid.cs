﻿namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Read-only grid control class.
    /// </summary>
    public interface IReadOnlyGrid : IPcfControl
    {
        /// <summary>
        /// Opens the record at the provided zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the record.</param>
        /// <returns>The record.</returns>
        Task<IEntityRecordPage> OpenRecordAsync(int index);

        /// <summary>
        /// Gets the column names on the view.
        /// </summary>
        /// <returns>The column names.</returns>
        Task<IEnumerable<string>> GetColumnNamesAsync();
    }
}