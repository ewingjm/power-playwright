namespace PowerPlaywright.TestApp.Model.Extensions
{
    using Bogus;

    /// <summary>
    /// Extensions for the <see cref="Randomizer"/> class.
    /// </summary>
    public static class RandomizerExtenions
    {
        /// <summary>
        /// Gets a random set of values from the specified enum type with an optional minimum number of values.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="randomizer">The randomizer.</param>
        /// <param name="min">The minimum number of values.</param>
        /// <param name="max">The maximum number of values. Defaults to the total number of enum values.</param>
        /// <returns>The collection of values.</returns>
        public static IEnumerable<TEnum> EnumValuesRange<TEnum>(this Randomizer randomizer, int min = 0, int? max = null)
        {
            return randomizer.EnumValuesRange<TEnum>(randomizer.Int(min, max ?? Enum.GetValues(typeof(TEnum)).Length));
        }
    }
}
