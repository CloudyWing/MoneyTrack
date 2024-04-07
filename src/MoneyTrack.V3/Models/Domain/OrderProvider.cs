using System.Linq.Expressions;
using CloudyWing.MoneyTrack.Models.Domain.Statements;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public class OrderProvider<T> {
        private readonly List<OrderByStatement> orderByStatements = [];

        public OrderProvider<T> Asc<TColumn>(Expression<Func<T, TColumn>> sortKey) {
            orderByStatements.Add(new OrderByStatement(new ColumnStatement<T, TColumn>(sortKey), OrderDirection.Asc));
            return this;
        }

        public OrderProvider<T> Desc<TColumn>(Expression<Func<T, TColumn>> sortKey) {
            orderByStatements.Add(new OrderByStatement(new ColumnStatement<T, TColumn>(sortKey), OrderDirection.Desc));
            return this;
        }

        public override string ToString() {
            return $"ORDER BY {string.Join(",", orderByStatements)}";
        }
    }
}
