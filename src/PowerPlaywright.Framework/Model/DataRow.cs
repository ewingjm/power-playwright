namespace PowerPlaywright.Framework.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a data row.
    /// </summary>
    public class DataRow : ReadOnlyDictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRow"/> class.
        /// </summary>
        /// <param name="data">A dictionary containing the data for the row.</param>
        public DataRow(IDictionary<string, string> data)
            : base(data)
        {
        }

        /// <summary>
        /// Get the column value as a raw string.
        /// </summary>
        /// <param name="columnName">The unique column name.</param>
        /// <returns>The raw string value.</returns>
        public string Get(string columnName)
        {
            var value = this[columnName];
            return value;
        }

        /// <summary>
        /// Determines if the row contains a column with the specified name.
        /// </summary>
        /// <param name="columnName">The unique column name.</param>
        /// <returns>true if the column exists; otherwise, false.</returns>
        public bool Contains(string columnName)
        {
            return this.Dictionary.ContainsKey(columnName);
        }
    }
}