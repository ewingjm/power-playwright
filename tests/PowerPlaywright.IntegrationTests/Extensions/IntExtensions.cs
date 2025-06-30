namespace PowerPlaywright.IntegrationTests.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="int"/> type.
    /// </summary>
    internal static class IntExtensions
    {
        /// <summary>
        /// Converts an integer minutes value to a string representation of a duration.
        /// </summary>
        /// <param name="value">The <see cref="int"/>.</param>
        /// <returns>The duration string.</returns>
        internal static string ToDurationString(this int value)
        {
            if (value < 60)
            {
                return $"{value} minute{(value > 1 ? "s" : string.Empty)}";
            }
            else if (value < 1440)
            {
                var hours = Math.Round(value / 60m, 2);
                return $"{(hours % 1 == 0 ? ((int)hours).ToString() : hours.ToString("0.##"))} minute{(hours > 1 ? "s" : string.Empty)}";
            }
            else
            {
                var days = Math.Round(value / 1440m, 2);
                return $"{(days % 1 == 0 ? ((int)days).ToString() : days.ToString("#,##0.##"))} day{(days > 1 ? "s" : string.Empty)}";
            }
        }
    }
}
