using System.Linq.Expressions;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// Represents a max statement in a SQL query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    /// <seealso cref="ColumnStatement{TEntity, TColumn}" />
    internal class MaxStatement<TEntity, TColumn>(Expression<Func<TEntity, TColumn>> columnExpression)
        : ColumnStatement<TEntity, TColumn>(columnExpression) {

        /// <inheritdoc/>
        public new string QuotedTableAliasAndColumnName => $"MAX({QuotedTableAliasAndColumnName})";
    }

    /// <summary>
    /// Represents a max statement in a SQL query with an alias.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    /// <typeparam name="TRecoed">The type of the recoed.</typeparam>
    /// <typeparam name="TAlias">The type of the alias.</typeparam>
    /// <seealso cref="ColumnStatement{TEntity, TColumn}" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="MaxStatement{TEntity, TColumn, TRecoed, TAlias}"/> class.
    /// </remarks>
    /// <param name="columnExpression">The column expression.</param>
    /// <param name="aliasExpression">The alias expression.</param>
    internal class MaxStatement<TEntity, TColumn, TRecoed, TAlias>(Expression<Func<TEntity, TColumn>> columnExpression, Expression<Func<TRecoed, TAlias>>? aliasExpression = null) : ColumnStatement<TEntity, TColumn, TRecoed, TAlias>(columnExpression, aliasExpression) {

        /// <inheritdoc/>
        public new string QuotedTableAliasAndColumnName {
            get {
                string result = $"MAX({QuotedTableAliasAndColumnName})";

                if (!string.IsNullOrWhiteSpace(ColumnAlias)) {
                    result += $" AS \"{ColumnAlias}\"";
                }

                return result;
            }
        }
    }
}
