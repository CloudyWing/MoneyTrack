namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// Defines the interface for the operator statement in a SQL query.
    /// </summary>
    public interface IOperatorStatement {
        /// <summary>
        /// Gets the column statement.
        /// </summary>
        /// <value>
        /// The column statement.
        /// </value>
        IColumnStatement ColumnStatement { get; }

        /// <summary>
        /// Gets the statement string.
        /// </summary>
        /// <value>
        /// The statement string.
        /// </value>
        string StatementString { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        IReadOnlyList<ParameterInfo> Parameters { get; }
    }
}
