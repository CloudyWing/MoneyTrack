using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// The <c>OrderByStatement</c> class represents a statement that is used to specify the sort order of the result set of a query.
    /// </summary>
    public class OrderByStatement {
        public OrderByStatement(IColumnStatement columnStatement, OrderDirection orderDirection) {
            ExceptionUtils.ThrowIfNull(() => columnStatement);

            ColumnStatement = columnStatement;
            OrderDirection = orderDirection;
        }

        /// <summary>
        /// Gets the column statement.
        /// </summary>
        /// <value>
        /// The column statement.
        /// </value>
        public IColumnStatement ColumnStatement { get; }

        /// <summary>
        /// Gets the order direction.
        /// </summary>
        /// <value>
        /// The order direction.
        /// </value>
        public OrderDirection OrderDirection { get; }

        /// <summary>
        /// Gets the string representation of the <c>OrderByStatement</c>.
        /// </summary>
        public string OrderByStatementString => $"{ColumnStatement} {OrderDirection.ToString().ToUpper()}";

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// Returns <c>OrderByStatementString</c>.
        /// </returns>
        public override string ToString() {
            return OrderByStatementString;
        }
    }
}
