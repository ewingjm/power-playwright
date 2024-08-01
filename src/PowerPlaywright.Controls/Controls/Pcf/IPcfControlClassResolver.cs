namespace PowerPlaywright.Model.Pcf
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Resolves a PCF control class.
    /// </summary>
    public interface IPcfControlClassResolver
    {
        /// <summary>
        /// Resolve a control class interface type to a control interface type.
        /// </summary>
        /// <param name="classType">The control class interface type.</param>
        /// <param name="controlTypes">The control interfaces types.</param>
        /// <returns>The resolved control interface type.</returns>
        Type Resolve(Type classType, IEnumerable<Type> controlTypes);
    }
}
