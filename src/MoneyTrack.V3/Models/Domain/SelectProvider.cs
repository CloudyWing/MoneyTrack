using System.Linq.Expressions;
using CloudyWing.MoneyTrack.Models.Domain.Statements;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public class SelectProvider<TEntity, TRecord> {
        private readonly List<IColumnStatement> statements = [];

        public SelectProvider<TEntity, TRecord> Column<TColumn, TAlias>(Expression<Func<TEntity, TColumn>> columnSelector, Expression<Func<TRecord, TAlias>> aliasSelector) {
            statements.Add(new ColumnStatement<TEntity, TColumn, TRecord, TAlias>(columnSelector, aliasSelector));
            return this;
        }

        public SelectProvider<TEntity, TRecord> Count<TColumn, TAlias>(Expression<Func<TEntity, TColumn>> columnSelector, Expression<Func<TRecord, TAlias>> aliasSelector) {
            statements.Add(new CountStatement<TEntity, TColumn, TRecord, TAlias>(columnSelector, aliasSelector));
            return this;
        }

        public SelectProvider<TEntity, TRecord> Count<TAlias>(Expression<Func<TRecord, TAlias>> aliasSelector) {
            statements.Add(new CountStatement<TEntity, string, TRecord, TAlias>(aliasSelector));
            return this;
        }

        public SelectProvider<TEntity, TRecord> Max<TColumn, TAlias>(Expression<Func<TEntity, TColumn>> columnSelector, Expression<Func<TRecord, TAlias>> aliasSelector) {
            statements.Add(new MaxStatement<TEntity, TColumn, TRecord, TAlias>(columnSelector, aliasSelector));
            return this;
        }

        public SelectProvider<TEntity, TRecord> IsNull<TColumn, TAlias>(Expression<Func<TEntity, TColumn>> columnSelector, string expr2, Expression<Func<TRecord, TAlias>> aliasSelector) {
            statements.Add(new IsNullStatement<TEntity, TColumn, TRecord, TAlias>(columnSelector, expr2, aliasSelector));
            return this;
        }


        public override string? ToString() {
            return string.Join(", ", statements);
        }
    }
}
