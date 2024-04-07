using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// Represents a statement for a column in a query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    /// <seealso cref="IColumnStatement" />
    internal class ColumnStatement<TEntity, TColumn> : IColumnStatement {
        private string tableAlias = "";
        private string columnName = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnStatement{TEntity, TColumn}"/> class.
        /// </summary>
        public ColumnStatement() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnStatement{TEntity, TColumn}"/> class.
        /// </summary>
        /// <param name="columnExpression">The column expression.</param>
        public ColumnStatement(Expression<Func<TEntity, TColumn>>? columnExpression) {
            Initialize(columnExpression);
        }

        /// <inheritdoc/>
        [AllowNull]
        public string TableAlias {
            get => tableAlias;
            protected set => tableAlias = value ?? "";
        }

        /// <inheritdoc/>
        [AllowNull]
        public string ColumnName {
            get => columnName;
            protected set => columnName = value ?? "";
        }

        /// <summary>
        /// Gets the name of the quoted table alias and column.
        /// </summary>
        /// <value>
        /// The name of the quoted table alias and column.
        /// </value>
        public string QuotedTableAliasAndColumnName => string.IsNullOrWhiteSpace(TableAlias)
                    ? $"{ColumnName}"
                    : $"{TableAlias}.{ColumnName}";

        /// <inheritdoc/>
        public virtual string ColumnStatementString => QuotedTableAliasAndColumnName;

        private void Initialize(Expression<Func<TEntity, TColumn>>? columnExpression) {
            if (columnExpression is not null) {
                (TableAlias, ColumnName) = StatementUtils.GetColumnInfo(columnExpression);
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// The <c>ColumnStatementString</c>.
        /// </returns>
        public override string ToString() {
            return ColumnStatementString;
        }
    }

    /// <summary>
    /// Represents a statement for a column in a query with an alias.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    /// <typeparam name="TRecoed">The type of the recoed.</typeparam>
    /// <typeparam name="TAlias">The type of the alias.</typeparam>
    /// <seealso cref="IColumnStatement" />
    internal class ColumnStatement<TEntity, TColumn, TRecoed, TAlias> : ColumnStatement<TEntity, TColumn>, IAliasColumnStatement {
        private string columnAlias = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnStatement{TEntity, TColumn, TRecoed, TAlias}"/> class.
        /// </summary>
        public ColumnStatement() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnStatement{TEntity, TColumn, TRecoed, TAlias}"/> class.
        /// </summary>
        /// <param name="columnExpression">The column expression.</param>
        /// <param name="aliasExpression">The alias expression.</param>
        public ColumnStatement(Expression<Func<TEntity, TColumn>>? columnExpression, Expression<Func<TRecoed, TAlias>>? aliasExpression = null)
            : base(columnExpression) {
            Initialize(aliasExpression);
        }

        /// <inheritdoc/>
        public string ColumnAlias {
            get => columnAlias;
            protected set => columnAlias = value ?? "";
        }

        /// <inheritdoc/>
        public override string ColumnStatementString => string.IsNullOrWhiteSpace(ColumnAlias)
                ? QuotedTableAliasAndColumnName
                : $"{QuotedTableAliasAndColumnName} AS \"{ColumnAlias}\"";

        private void Initialize(Expression<Func<TRecoed, TAlias>>? aliasExpression = null) {
            if (aliasExpression is not null) {
                ColumnAlias = ExpressionUtils.GetMember(aliasExpression).First();
            }
        }
    }
}
