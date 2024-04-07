using System.Linq.Expressions;
using System.Reflection;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Dapper;

namespace CloudyWing.MoneyTrack.Models.Domain.Statements {
    internal static class StatementUtils {
        public static (string? TableAlias, string? ColumnName) GetColumnInfo<TEntity, TColumn>(Expression<Func<TEntity, TColumn>> expression) {
            IEnumerable<string> columnKeys = ExpressionUtils.GetMember(expression);

            switch (columnKeys.Count()) {
                case 1:
                    PropertyInfo prop = typeof(TEntity).GetProperty(columnKeys.First())!;
                    return (null, GetColumnName(prop));
                case 2:
                    PropertyInfo tableProp = typeof(TEntity).GetProperty(columnKeys.First())!;
                    PropertyInfo colProp = tableProp.PropertyType.GetProperty(columnKeys.ElementAt(1))!;
                    return (tableProp.Name, GetColumnName(colProp));
                default:
                    return (null, null);

            }
        }

        private static string GetColumnName(PropertyInfo prop) {
            ColumnAttribute? attr = prop.GetCustomAttribute<ColumnAttribute>();
            return attr is null
                ? prop.Name
                : attr.Name;
        }
    }
}
