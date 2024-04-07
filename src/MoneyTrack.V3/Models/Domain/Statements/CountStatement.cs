using System.Linq.Expressions;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// Represents a count statement in a SQL query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    /// <seealso cref="ColumnStatement{TEntity, TColumn}" />
    internal class CountStatement<TEntity, TColumn> : ColumnStatement<TEntity, TColumn> {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountStatement{TEntity, TColumn}"/> class.
        /// </summary>
        public CountStatement() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountStatement{TEntity, TColumn}"/> class.
        /// </summary>
        /// <param name="columnExpression">The column expression.</param>
        public CountStatement(Expression<Func<TEntity, TColumn>>? columnExpression)
            : base(columnExpression) { }

        /// <inheritdoc/>
        public override string ColumnStatementString => string.IsNullOrWhiteSpace(ColumnName)
                ? "COUNT(1)"
                : $"COUNT({QuotedTableAliasAndColumnName})";
    }

    /// <summary>
    /// Represents a statement for counting the values in a column of a query with an alias.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    /// <typeparam name="TRecoed">The type of the recoed.</typeparam>
    /// <typeparam name="TAlias">The type of the alias.</typeparam>
    /// <seealso cref="ColumnStatement{TEntity, TColumn}" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="CountStatement{TEntity, TColumn, TRecoed, TAlias}"/> class.
    /// </remarks>
    /// <param name="columnExpression">The column expression.</param>
    /// <param name="aliasExpression">The alias expression.</param>
    internal class CountStatement<TEntity, TColumn, TRecoed, TAlias>(Expression<Func<TEntity, TColumn>>? columnExpression, Expression<Func<TRecoed, TAlias>>? aliasExpression = null) : ColumnStatement<TEntity, TColumn, TRecoed, TAlias>(columnExpression, aliasExpression) {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountStatement{TEntity, TColumn, TRecoed, TAlias}"/> class.
        /// </summary>
        /// <param name="aliasExpression">The alias expression.</param>
        public CountStatement(Expression<Func<TRecoed, TAlias>> aliasExpression) : this(null, aliasExpression) { }

        /// <inheritdoc/>
        public override string ColumnStatementString {
            get {
                string countColumn = string.IsNullOrWhiteSpace(ColumnName)
                    ? "COUNT(1)"
                    : $"COUNT({QuotedTableAliasAndColumnName})";

                return string.IsNullOrWhiteSpace(ColumnAlias)
                    ? countColumn
                    : $"{countColumn} AS \"{ColumnAlias}\"";
            }
        }
    }
}
