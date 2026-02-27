namespace PowerPlaywright.Framework.Model
{
    /// <summary>
    /// Specifies a column sort specification.
    /// </summary>
    public class ColumnSortSpec
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnSortSpec"/> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="order">The column sort order.</param>
        public ColumnSortSpec(string columnName, ColumnSortOrder order)
        {
            this.ColumnName = columnName;
            this.Order = order;
        }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the column sort order.
        /// </summary>
        public ColumnSortOrder Order { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ColumnSortSpec spec &&
                   this.ColumnName == spec.ColumnName &&
                   this.Order == spec.Order;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.ColumnName.GetHashCode() ^ this.Order.GetHashCode();
        }
    }
}
