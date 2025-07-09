namespace PowerPlaywright.Framework.Controls.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// A main form header.
    /// </summary>
    [PlatformControl]
    public interface IMainFormHeader : IPlatformControl, IAsyncDisposable
    {
        /// <summary>
        /// Gets all fields on the header.
        /// </summary>
        /// <returns>The fields.</returns>
        Task<IEnumerable<IField>> GetFieldsAsync();

        /// <summary>
        /// Gets a field on the header with a known child control type.
        /// </summary>
        /// <typeparam name="TControl">The child control type.</typeparam>
        /// <param name="name">The field name.</param>
        /// <returns>The field.</returns>
        IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl;

        /// <summary>
        /// Gets a field on the header.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>The field.</returns>
        IField GetField(string name);
    }
}
