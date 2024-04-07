namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// Defines the interface for a column statement.
    /// </summary>
    public interface IColumnStatement {
        /// <summary>
        /// Gets the table alias.
        /// </summary>
        /// <value>
        /// The table alias.
        /// </value>
        public string TableAlias { get; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the column statement.
        /// </summary>
        /// <value>
        /// The column statement.
        /// </value>
        public string ColumnStatementString { get; }
    }
}
