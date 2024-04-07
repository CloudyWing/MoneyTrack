using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    /// <summary>
    /// The <c>OperatorColumn</c> class provides column operator value statements, such as greater than, less than, greater than or equal to, and less than or equal to.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column.</typeparam>
    public class OperatorColumn<TEntity, TColumn> {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorColumn{TEntity, TColumn}"/> class.
        /// </summary>
        /// <param name="columnStatement">The column statement.</param>
        /// <exception cref="ExceptionUtils"></exception>
        public OperatorColumn(IColumnStatement columnStatement) {
            ExceptionUtils.ThrowIfNull(() => columnStatement);

            ColumnStatement = columnStatement;
        }

        /// <summary>
        /// Gets the column statement.
        /// </summary>
        /// <value>
        /// The column statement.
        /// </value>
        public IColumnStatement ColumnStatement { get; }

        #region 定義 == 和 != 太危險了，所以只定義 > < >= <=
        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static IOperatorStatement operator >(OperatorColumn<TEntity, TColumn> left, TColumn right) {
            return left.Greater(right);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static IOperatorStatement operator <(OperatorColumn<TEntity, TColumn> left, TColumn right) {
            return left.Less(right);
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static IOperatorStatement operator >=(OperatorColumn<TEntity, TColumn> left, TColumn right) {
            return left.GreaterOrEqual(right);
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static IOperatorStatement operator <=(OperatorColumn<TEntity, TColumn> left, TColumn right) {
            return left.LessOrEqual(right);
        }
        #endregion
    }
}
