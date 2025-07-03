namespace PowerPlaywright.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="Type"/> class.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Gets whether a generic type is assignable from another type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="c">The type to compare.</param>
        /// <returns>A boolean indicating whether the type is assignable.</returns>
        public static bool IsGenericAssignableFrom(this Type type, Type c)
        {
            foreach (var i in c.GetInterfaces())
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == type)
                {
                    return true;
                }
            }

            while (c != null && c != typeof(object))
            {
                if (c.IsGenericType && c.GetGenericTypeDefinition() == type)
                {
                    return true;
                }

                c = c.BaseType;
            }

            return false;
        }
    }
}
