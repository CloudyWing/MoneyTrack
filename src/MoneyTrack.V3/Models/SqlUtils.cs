using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CloudyWing.MoneyTrack.Models {
    public static class SqlUtils {
        public static int GetFirstItemOnPage(int pageNumber, int pageSize) {
            return ((pageNumber - 1) * pageSize) + 1;
        }

        public static int GetLastItemOnPage(int pageNumber, int pageSize) {
            return pageNumber * pageSize;
        }

        public static string RemoveNoLockHint(string commandText) {
            return Regex.Replace(
                commandText, @"\bWITH\s*\(\s*NOLOCK\s*\)\s*", " ", RegexOptions.IgnoreCase
            );
        }

        public static string RemoveOrderByClause(string orderBy) {
            return Regex.Replace(orderBy, @"\bORDER\s+BY\b", "", RegexOptions.IgnoreCase);
        }

        public static string EnsureRecompileOption(string commandText) {
            if (!Regex.IsMatch(commandText, @".+OPTION\s*\(\s*RECOMPILE\s*\).*$", RegexOptions.IgnoreCase)) {
                return commandText += " OPTION (RECOMPILE)";
            }
            return commandText;
        }

        public static string GetTableName<TEntity>() where TEntity : class {
            var entityType = typeof(TEntity);
            var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();
            return tableAttribute != null ? tableAttribute.Name : entityType.Name;
        }

        public static string GetPrimaryKeyName<TEntity>() where TEntity : class {
            var entityType = typeof(TEntity);
            var properties = entityType.GetProperties();
            foreach (var property in properties) {
                if (Attribute.IsDefined(property, typeof(KeyAttribute))) {
                    return property.Name;
                }
            }

            throw new InvalidOperationException("Primary key not found for the entity.");
        }
    }
}
