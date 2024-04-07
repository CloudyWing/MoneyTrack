using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using CloudyWing.MoneyTrack.Models;
using CloudyWing.MoneyTrack.Models.Enumerations;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// Utility class for working with databases.
    /// </summary>
    public static class DatabaseUtils {
        /// <summary>
        /// Creates a new SqlConnection object.
        /// </summary>
        /// <param name="extra">Optional extra parameter to append to the connection string.</param>
        /// <returns>A new SqlConnection object.</returns>
        public static SqlConnection CreateConnection(string extra = null) {
            StringBuilder sb = new StringBuilder();
            sb.Append(WebConfigurationManager.ConnectionStrings["Default"]);

            if (!string.IsNullOrEmpty(extra)) {
                sb.Append(extra);
            }

            SqlConnection conn = new SqlConnection(sb.ToString());
            conn.Open();

            return conn;
        }

        /// <summary>
        /// Creates a new SqlParameter object with the specified key, field, and value.
        /// </summary>
        /// <param name="key">The key for the SqlParameter object.</param>
        /// <param name="field">The TableField object containing information about the database field.</param>
        /// <param name="value">The value for the SqlParameter object.</param>
        /// <returns>A new SqlParameter object.</returns>
        public static SqlParameter CreateParameter(string key, TableField field, object value) {
            value = value ?? DBNull.Value;
            key = key.Replace("[", "").Replace("]", "");
            SqlParameter parameter;
            switch (field.DbType) {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                    parameter = new SqlParameter(key, field.DbType, field.Size);
                    break;
                case SqlDbType.Decimal:
                    parameter = new SqlParameter(key, field.DbType) {
                        Precision = field.Precision,
                        Scale = field.Scale
                    };
                    break;
                default:
                    parameter = new SqlParameter(key, field.DbType);
                    break;
            }
            parameter.Value = value;
            parameter.IsNullable = field.AllowDBNull;

            return parameter;
        }

        /// <summary>
        /// Adds a WHERE clause to the specified SqlWhereHelper object with the specified column name, key, field, and value.
        /// </summary>
        /// <param name="colnum">The name of the column to compare.</param>
        /// <param name="key">The key to use for the parameter.</param>
        /// <param name="field">The TableField object containing information about the database field.</param>
        /// <param name="wheres">The SqlWhereHelper object to add the WHERE clause to.</param>
        /// <param name="command">The SqlCommand object to add the parameter to.</param>
        /// <param name="value">The value to compare the column to.</param>
        public static void AddWhere(string colnum, string key, TableField field, SqlWhereHelper wheres, SqlCommand command, object value) {
            object[] values = new object[] {
                value
            };
            AddWhere(colnum, key, field, wheres, command, values, CompareMode.Equal);
        }

        /// <summary>
        /// Adds a WHERE clause to the specified SqlWhereHelper object with the specified column name, key, field, and array of values.
        /// </summary>
        /// <param name="colnum">The name of the column to compare.</param>
        /// <param name="key">The key to use for the parameters.</param>
        /// <param name="field">The TableField object containing information about the database field.</param>
        /// <param name="wheres">The SqlWhereHelper object to add the WHERE clause to.</param>
        /// <param name="command">The SqlCommand object to add the parameters to.</param>
        /// <param name="values">The IEnumerable of values to compare the column to.</param>
        public static void AddWhere(string colnum, string key, TableField field, SqlWhereHelper wheres, SqlCommand command, IEnumerable<object> values) {
            AddWhere(colnum, key, field, wheres, command, values, CompareMode.Equal);
        }

        /// <summary>
        /// Adds a WHERE clause to the specified SqlWhereHelper object with the specified column name, key, field, array of values, and comparison option.
        /// </summary>
        /// <param name="colnum">The name of the column to compare.</param>
        /// <param name="key">The key to use for the parameters.</param>
        /// <param name="field">The TableField object containing information about the database field.</param>
        /// <param name="wheres">The SqlWhereHelper object to add the WHERE clause to.</param>
        /// <param name="command">The SqlCommand object to add the parameters to.</param>
        /// <param name="values">The IEnumerable of values to compare the column to.</param>
        /// <param name="option">The comparison option, such as Equals, NotEqual, Like, NotLike, etc.</param>
        public static void AddWhere(string colnum, string key, TableField field, SqlWhereHelper wheres, SqlCommand command, IEnumerable<object> values, CompareMode option) {
            List<string> subWheres = new List<string>();
            List<object> _values = values.ToList();
            bool hasRemoveNull = false;
            if (_values.Contains(DBNull.Value)) {
                subWheres.Add(
                    string.Format(
                        "{0} {1} NULL",
                        colnum,
                        (option != CompareMode.NotEqual && option != CompareMode.NotLike) ? "IS" : "IS NOT"
                    )
                );
                _values.Remove(DBNull.Value);
                hasRemoveNull = true;
            }

            if (_values.Count == 1) {
                subWheres.Add($"{colnum} {option} {key}");
                command.Parameters.Add(
                    CreateParameter(
                        key,
                        field,
                        string.Format(
                            "{0}{1}{0}",
                            (option == CompareMode.Like || option == CompareMode.NotLike) ? "%" : "",
                            _values[0]
                        )
                    )
                );

            } else if (_values.Count > 1) {
                int idx = 0;
                List<string> keys = new List<string>();
                foreach (object value in _values) {
                    string newKey = string.Format("{0}_{1}", key, idx++);
                    keys.Add(newKey);
                    command.Parameters.Add(CreateParameter(newKey, field, value));
                }
                subWheres.Add(
                    string.Format(
                        "{0} {1} ({2})",
                        colnum,
                         (option != CompareMode.NotEqual && option != CompareMode.NotLike) ? "IN" : "NOT IN",
                        string.Join(", ", keys.ToArray())
                    )
                );
            } else if (!hasRemoveNull) {
                switch (field.DbType) {
                    case SqlDbType.BigInt:
                    case SqlDbType.Decimal:
                    case SqlDbType.Float:
                    case SqlDbType.Int:
                    case SqlDbType.Money:
                    case SqlDbType.Real:
                    case SqlDbType.SmallInt:
                    case SqlDbType.SmallMoney:
                        subWheres.Add(
                            (option == CompareMode.NotEqual || option == CompareMode.NotLike) ?
                                "1 = 1" : " 1 <> 1"
                        );
                        break;
                    default:
                        subWheres.Add($"{colnum} {option} ''");
                        break;
                }
            }
            wheres.Add(
                string.Format(
                    "{0}{1}{2}",
                    subWheres.Count > 1 ? "(" : "",
                    string.Join(" OR ", subWheres.ToArray()),
                    subWheres.Count > 1 ? ")" : ""
                )
            );
        }
    }
}
