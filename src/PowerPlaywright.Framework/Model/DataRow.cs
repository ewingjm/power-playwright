namespace PowerPlaywright.Framework.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a data row.
    /// </summary>
    public class DataRow : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IReadOnlyDictionary<string, string> data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRow"/> class.
        /// </summary>
        /// <param name="data">A dictionary containing the data for the row.</param>
        public DataRow(IDictionary<string, string> data)
        {
            this.data = new Dictionary<string, string>(data, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the value associated with the specified column name in the data row.
        /// </summary>
        /// <param name="columnName">The name of the column to retrieve the value for.</param>
        /// <returns>The value of the specified column.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the specified column name does not exist in the data row.</exception>
        public string this[string columnName]
        {
            get
            {
                if (this.data.TryGetValue(columnName, out var value))
                {
                    return value;
                }

                throw new KeyNotFoundException($"Column '{columnName}' not found in data row.");
            }
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
            return this.data.ContainsKey(columnName);
        }

        /// <summary>
        /// Attempts to get the column value as a raw string.
        /// </summary>
        /// <param name="columnName">The unique column name.</param>
        /// <param name="value">The column value.</param>
        /// <returns>true if the column is found; otherwise false.</returns>
        public bool TryGetValue(string columnName, out string value)
        {
            value = default;
            if (this.data.TryGetValue(columnName, out var stringValue))
            {
                try
                {
                    value = stringValue;
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of key-value pairs.
        /// </summary>
        /// <returns>An enumerator for the collection of key-value pairs.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}