namespace PowerPlaywright.IntegrationTests.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="DateTime"/> class.
    /// </summary>
    internal static class DateTimeExtensions
    {
        /// <summary>
        /// Removes the ticks from a <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> instance.</param>
        /// <returns>A new <see cref="DateTime"/> instance without the ticks.</returns>
        public static DateTime WithoutSeconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }
    }
}
