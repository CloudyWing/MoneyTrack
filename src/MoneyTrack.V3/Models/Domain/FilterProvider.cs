using System.Linq.Expressions;
using CloudyWing.MoneyTrack.Models.Domain.Statements;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public class FilterProvider<T> {
        public OperatorColumn<T, TColumn> Column<TColumn>(Expression<Func<T, TColumn>> filterKey) {
            return new OperatorColumn<T, TColumn>(new ColumnStatement<T, TColumn>(filterKey));
        }

        public OperatorColumn<T, TColumn> Max<TColumn>(Expression<Func<T, TColumn>> filterKey) {
            return new OperatorColumn<T, TColumn>(new MaxStatement<T, TColumn>(filterKey));
        }

        public OperatorColumn<T, TColumn> Count<TColumn>(Expression<Func<T, TColumn>> filterKey) {
            return new OperatorColumn<T, TColumn>(new CountStatement<T, TColumn>(filterKey));
        }

        public OperatorColumn<T, TColumn> IsNull<TColumn>(Expression<Func<T, TColumn>> filterKey, string expr2) {
            return new OperatorColumn<T, TColumn>(new IsNullStatement<T, TColumn>(filterKey, expr2));
        }
    }
}
