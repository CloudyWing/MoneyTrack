namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// Defines the interface for a column statement with alias.
    /// </summary>
    /// <seealso cref="IColumnStatement" />
    internal interface IAliasColumnStatement : IColumnStatement {
        /// <summary>
        /// Gets the column alias.
        /// </summary>
        /// <value>
        /// The column alias.
        /// </value>
        public string ColumnAlias { get; }
    }
}
