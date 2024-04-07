using System.Linq.Expressions;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// Represents a nvl statement in a SQL query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    /// <seealso cref="ColumnStatement{TEntity, TColumn}" />
    internal class IsNullStatement<TEntity, TColumn> : ColumnStatement<TEntity, TColumn> {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNullStatement{TEntity, TColumn}"/> class.
        /// </summary>
        /// <param name="columnExpression">The column expression.</param>
        /// <param name="expr2">The expr2.</param>
        public IsNullStatement(Expression<Func<TEntity, TColumn>> columnExpression, string expr2)
            : base(columnExpression) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => expr2);

            Expr2 = expr2;
        }

        /// <summary>
        /// Gets or sets the expr2.
        /// </summary>
        /// <value>
        /// The expr2.
        /// </value>
        public string Expr2 { get; set; }

        /// <inheritdoc/>
        public override string ColumnStatementString => $"ISNULL({QuotedTableAliasAndColumnName}, {Expr2})";
    }

    /// <summary>
    /// Represents a nvl statement in a SQL query with an alias.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    /// <typeparam name="TRecoed">The type of the recoed.</typeparam>
    /// <typeparam name="TAlias">The type of the alias.</typeparam>
    /// <seealso cref="ColumnStatement{TEntity, TColumn}" />
    internal class IsNullStatement<TEntity, TColumn, TRecoed, TAlias> : ColumnStatement<TEntity, TColumn, TRecoed, TAlias> {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNullStatement{TEntity, TColumn, TRecoed, TAlias}"/> class.
        /// </summary>
        /// <param name="columnExpression">The column expression.</param>
        /// <param name="expr2">The expr2.</param>
        /// <param name="aliasExpression">The alias expression.</param>
        public IsNullStatement(Expression<Func<TEntity, TColumn>> columnExpression, string expr2, Expression<Func<TRecoed, TAlias>>? aliasExpression = null)
            : base(columnExpression, aliasExpression) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => expr2);

            Expr2 = expr2;
        }

        public string Expr2 { get; }

        /// <inheritdoc/>
        public override string ColumnStatementString {
            get {
                string result = $"ISNULL({QuotedTableAliasAndColumnName}, {Expr2}";

                if (!string.IsNullOrWhiteSpace(ColumnAlias)) {
                    result += $" AS \"{ColumnAlias}\"";
                }

                return result;
            }
        }
    }
}
