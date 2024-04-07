using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Enumerations;

namespace CloudyWing.MoneyTrack.Models {
    /// <summary>
    /// The DataUpdater class is responsible for modifying data in a SQL database table.
    /// </summary>
    public sealed class DataUpdater {
        private readonly DataValueCollection columns = new DataValueCollection();
        private SqlConnection conn;
        private DataModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataUpdater"/> class.
        /// </summary>
        /// <param name="model">The data model.</param>
        public DataUpdater(DataModel model) {
            Model = model;
        }


        private enum Command {
            Insert,
            Update,
            Delete
        }

        /// <summary>
        /// Gets or sets the data model.
        /// </summary>
        public DataModel Model {
            set {
                model = value;
                ReSet();
            }
            get => model;
        }

        /// <summary>
        /// Gets the SQL connection.
        /// </summary>
        public SqlConnection Connection {
            get {
                if (conn is null) {
                    conn = DatabaseUtils.CreateConnection();
                }

                if (conn.State == ConnectionState.Broken) {
                    conn.Close();
                }

                if (conn.State == ConnectionState.Closed) {
                    conn.Dispose();
                    conn = null;
                    conn = DatabaseUtils.CreateConnection();
                }
                return conn;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance allows multiple data to be deleted or modified.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets the identity.
        /// </summary>
        public int Identity { get; private set; }

        /// <summary>
        /// Adds a column to the collection.
        /// </summary>
        /// <param name="field">The table field.</param>
        private void AddColumn(TableField field) {
            DataValue dc = new DataValue(field);
            columns.AddOrReplace(dc);
        }

        private string GetWhereString() {
            SqlWhereHelper wheres = new SqlWhereHelper();

            foreach (DataValue column in columns) {
                TableField field = column.Field;
                object value = column.Value;
                object multiValue = column.MultiValue;
                bool useMulti = column.UseMulti;
                CompareMode mode = column.Mode;
                string key = "@Unique_" + field.Name;

                if (IsUnique) {
                    if (field.IsPrimaryKey) {
                        if (value == DBNull.Value) {
                            wheres.Add(string.Format("{0} IS NULL", field.NameWithBrackets));
                        } else {
                            wheres.Add(string.Format("{0} = {1}", field.NameWithBrackets, key));
                        }
                    }
                } else {
                    if (useMulti && multiValue != null) {
                        if (multiValue == DBNull.Value) {
                            wheres.Add(string.Format(
                                "{0} {1} NULL",
                                field.NameWithBrackets,
                                (mode != CompareMode.NotEqual && mode != CompareMode.NotLike) ? "IS" : "IS NOT"
                            ));
                        } else {
                            wheres.Add(string.Format("{0} {1} {2}", field.NameWithBrackets, mode, key));
                        }
                    }
                }
            }

            return wheres.WhereAndString;
        }

        private List<SqlParameter> GetWhereParameters() {
            List<SqlParameter> collection = new List<SqlParameter>();
            foreach (DataValue column in columns) {
                TableField field = column.Field;
                object value = column.Value;
                object multiValue = column.MultiValue;
                bool useMulti = column.UseMulti;
                string key = "@Unique_" + field.Name;

                if (IsUnique) {
                    if (field.IsPrimaryKey) {
                        collection.Add(DatabaseUtils.CreateParameter(key, field, value));
                    }
                } else {
                    if (useMulti) {
                        collection.Add(DatabaseUtils.CreateParameter(key, field, multiValue));
                    }
                }
            }
            return collection;
        }

        private SqlCommand GetInsertCommand() {
            List<string> colnums = new List<string>();
            List<string> values = new List<string>();
            SqlCommand cmd = new SqlCommand();
            string identityKey = "identity" + new Random().Next(100, 999);
            string identitySql = string.Empty;

            foreach (DataValue column in columns) {
                TableField field = column.Field;
                object value = column.Value;
                string name = field.NameWithBrackets;
                string key = "@" + field.Name;
                bool isPK = field.IsPrimaryKey;
                bool isIdentity = field.IsIdentity;

                if (!isIdentity && (isPK || value != null)) {
                    colnums.Add(name);
                    values.Add(key);
                    cmd.Parameters.Add(DatabaseUtils.CreateParameter(key, field, value));
                }

                if (isIdentity) {
                    SqlParameter p = new SqlParameter(identityKey, SqlDbType.Int) {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(p);
                    identitySql = string.Format(
                        "; SET {0} = IDENT_CURRENT('{1}');",
                        "@" + identityKey,
                        Model.TableName
                    );
                }
            }

            if (colnums.Count == 0) {
                throw new KeyNotFoundException("未填入新增欄位");
            }
            string sql = string.Format(
                "INSERT INTO {0} ({1}) VALUES({2}) {3}",
                Model.TableName,
                string.Join(", ", colnums.ToArray()), string.Join(", ", values.ToArray()),
                identitySql
            );
            cmd.CommandText = sql;
            return cmd;
        }

        private SqlCommand GetUpdateCommand() {
            List<string> colnums = new List<string>();
            SqlCommand cmd = new SqlCommand();
            StringBuilder sql = new StringBuilder();
            List<SqlParameter> whereParameters = GetWhereParameters();

            foreach (DataValue column in columns) {
                TableField field = column.Field;
                object newValue = column.NewValue;
                string name = field.NameWithBrackets;
                string key = "@" + field.Name;
                bool useNew = column.UseNew;

                if (useNew) {
                    bool replaceFlag = false;
                    string replaceStr = string.Empty;
                    if (newValue is string) {
                        foreach (DataValue innerColumn in columns) {
                            string replaceName = innerColumn.Field.Name;
                            string replaceKey = string.Format("{{{0}}}", replaceName);
                            replaceStr = newValue.ToString();
                            if (-1 != replaceStr.IndexOf(replaceKey)) {
                                replaceStr = replaceStr.Replace(replaceKey, replaceName);
                                replaceFlag = true;
                            }
                        }
                    }
                    if (replaceFlag) {
                        colnums.Add(string.Format("{0} = {1}", name, replaceStr));
                    } else {
                        colnums.Add(string.Format("{0} = {1}", name, key));
                        cmd.Parameters.Add(DatabaseUtils.CreateParameter(key, field, newValue));
                    }
                }
            }

            if (colnums.Count == 0) {
                throw new KeyNotFoundException("未填入修改欄位");
            }

            sql.AppendFormat("UPDATE {0} SET {1} {2} ", Model.TableName, string.Join(", ", colnums.ToArray()), GetWhereString());
            foreach (SqlParameter _parameter in whereParameters) {
                cmd.Parameters.Add(_parameter);
            }
            cmd.CommandText = sql.ToString();

            return cmd;
        }

        private SqlCommand GetDeleteCommand() {
            SqlCommand cmd = new SqlCommand();
            List<SqlParameter> whereParameters = GetWhereParameters();

            cmd.CommandText = string.Format("DELETE FROM {0} {1}", Model.TableName, GetWhereString());
            foreach (SqlParameter _parameter in whereParameters) {
                cmd.Parameters.Add(_parameter);
            }
            return cmd;
        }

        /// <summary>
        /// Determines whether the current state of the columns collection corresponds to an existing row in the table.
        /// </summary>
        /// <returns>true if a row exists in the table corresponding to the current state of the columns collection; otherwise, false.</returns>
        public bool IsExist() {
            foreach (DataValue col in columns) {
                TableField field = col.Field;
                // 識別欄位沒設值一率當不存在
                if (field.IsIdentity && col.Value is null) {
                    return false;
                }
            }

            using (SqlConnection conn = Connection) {
                string sql = string.Format("SELECT COUNT(*) FROM {0} {1}", Model.TableName, GetWhereString());
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddRange(GetWhereParameters().ToArray());
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// Inserts a new row into the table based on the current state of the columns collection.
        /// </summary>
        /// <returns>The number of rows affected by the insert operation.</returns>
        public int Insert() {
            if (!IsExist()) {
                Check(Command.Insert);
                using (SqlConnection conn = Connection)
                using (SqlCommand cmd = GetInsertCommand()) {
                    cmd.Connection = conn;
                    int count = cmd.ExecuteNonQuery();
                    foreach (SqlParameter p in cmd.Parameters) {
                        if (Regex.IsMatch(p.ParameterName, @"identity\d{3}", RegexOptions.IgnoreCase)) {
                            Identity = Convert.ToInt32(p.Value);
                        }
                    }
                    return count;
                }
            }
            return 0;
        }

        /// <summary>
        /// Updates the row in the table corresponding to the current state of the columns collection.
        /// </summary>
        /// <returns>The number of rows affected by the update operation.</returns>
        public int Update() {
            Check(Command.Update);
            using (SqlConnection conn = Connection)
            using (SqlCommand cmd = GetUpdateCommand()) {
                cmd.Connection = conn;
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the row in the table corresponding to the current state of the columns collection.
        /// </summary>
        /// <returns>The number of rows affected by the delete operation.</returns>
        public int Delete() {
            Check(Command.Delete);
            using (SqlConnection conn = Connection)
            using (SqlCommand cmd = GetDeleteCommand()) {
                cmd.Connection = conn;
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts a new row into the table based on the current state of the columns collection if no corresponding row exists; otherwise, updates the corresponding row.
        /// </summary>
        /// <returns>The number of rows affected by the insert or update operation.</returns>
        public int InsertOrUpdate() {
            return IsExist()
                ? Update() : Insert();
        }

        /// <summary>
        /// Sets the value of a column in the columns collection to the specified value.
        /// </summary>
        /// <param name="field">The table field to set.</param>
        /// <param name="value">The value to set the column to.</param>
        public void SetValue(TableField field, object value) {
            if (value is null) {
                return;
            }
            VerifyModel(field);

            DataValue dc = columns[field];
            dc.Value = value;
            columns[field] = dc;
        }

        /// <summary>
        /// Sets the new value of a column in the columns collection to the specified value.
        /// </summary>
        /// <param name="field">The table field to set.</param>
        /// <param name="value">The value to set the column to.</param>
        public void SetNewValue(TableField field, object value) {
            if (value is null) {
                return;
            }
            VerifyModel(field);

            DataValue dc = columns[field];
            dc.NewValue = value;
            columns[field] = dc;
        }

        /// <summary>
        /// Sets the value of a field for multiple updates or deletions with an equal comparison mode.
        /// </summary>
        /// <param name="field">The field to set the value for.</param>
        /// <param name="value">The value to set.</param>
        public void SetMultiValue(TableField field, object value) {
            SetMultiValue(field, CompareMode.Equal, value);
        }

        /// Sets the value of a field for multiple updates or deletions with the specified comparison mode.
        /// </summary>
        /// <param name="field">The field to set the value for.</param>
        /// <param name="mode">The comparison mode to use.</param>
        /// <param name="value">The value to set.</param>
        public void SetMultiValue(TableField field, CompareMode mode, object value) {
            if (value is null) {
                return;
            }
            VerifyModel(field);

            DataValue dc = columns[field];
            dc.MultiValue = value;
            dc.Mode = mode;
            columns[field] = dc;
        }

        private void Check(Command useCommand) {
            bool hasCondition = false;
            bool hasModify = false;

            foreach (DataValue col in columns) {
                TableField field = col.Field;
                bool isPK = field.IsPrimaryKey;
                bool isIdentity = field.IsIdentity;
                bool allowNull = field.AllowDBNull;
                bool hasDefault = field.HasDefault;
                bool useNew = col.UseNew;
                bool useMulti = col.UseMulti;
                object value = col.Value;
                object newValue = col.NewValue;
                object multiValue = col.MultiValue;

                if ((isIdentity || hasDefault) && useCommand == Command.Insert) {
                    continue;
                }

                if (isPK || !allowNull) {

                    if (useCommand == Command.Insert && (value == DBNull.Value || value is null)) {
                        throw new ArgumentException($"Insert 時，{field.Name}為必填，不可為 Null 或 {nameof(DBNull)}。", field.Name);
                    }
                    if (useCommand == Command.Update && useNew && newValue == DBNull.Value) {
                        throw new ArgumentException($"Update 時，{field.Name}為必填，不可為 Null或 {nameof(DBNull)}。", field.Name);
                    }
                    if (useCommand != Command.Insert && !IsUnique && useMulti && multiValue == DBNull.Value) {
                        throw new ArgumentException($"多筆 Insert 或 Update 時，{field.Name}為必填，不可為 Null 或 {nameof(DBNull)}。", field.Name);
                    }
                }

                if (useCommand == Command.Update) {
                    if (useNew) {
                        hasModify = true;
                    }
                }

                if (useCommand != Command.Insert) {
                    if (IsUnique) {
                        if (isPK && value != null) {
                            hasCondition = true;
                        }
                    } else {
                        if (useMulti) {
                            hasCondition = true;
                        }
                    }
                }
            }

            if (useCommand == Command.Update && !hasModify) {
                throw new KeyNotFoundException("至少輸入一個異動值。");
            }

            if (useCommand != Command.Insert && !hasCondition) {
                throw new KeyNotFoundException("至少輸入一個條件。");
            }
        }

        private void VerifyModel(TableField field) {
            if (field.Model != Model) {
                throw new ArgumentException($"{nameof(TableField)} 的 {nameof(DataModel)} 錯誤，請輸入{Model.GetType()}的 {nameof(TableField)}。");
            }
            if (!columns.Contains(field)) {
                throw new KeyNotFoundException($"設定 {nameof(TableField)} 不存在。");
            }
        }

        /// <summary>
        /// Resets the columns of the data updater to the TableFields of its model and sets IsUnique to true.
        /// </summary>
        public void ReSet() {
            columns.Clear();
            IEnumerable<TableField> tfs = Model.TableFields;
            foreach (TableField tf in tfs) {
                AddColumn(tf);
            }

            IsUnique = true;
        }

        private struct DataValue {
            private object newValue;
            private object multiValue;
            private CompareMode mode;

            public DataValue(TableField field) : this() {
                Field = field;
            }

            public TableField Field { get; set; }

            public object Value { get; set; }

            public object NewValue {
                get => newValue;
                set {
                    newValue = value;
                    UseNew = value != null;
                }
            }

            public bool UseNew { get; private set; }

            public object MultiValue {
                get => multiValue;
                set {
                    multiValue = value;
                    UseMulti = value != null;
                }
            }

            public bool UseMulti { get; private set; }

            public CompareMode Mode {
                get {
                    if (mode is null) {
                        mode = CompareMode.Equal;
                    }
                    return mode;
                }
                set => mode = value;
            }
        }

        private class DataValueCollection : KeyedCollection<TableField, DataValue> {
            public new DataValue this[TableField field] {
                get => base[field];
                set {
                    for (int i = 0; i < Count; i++) {
                        if (GetKeyForItem(Items[i]).Equals(field)) {
                            SetItem(i, value);
                        }
                    }
                }
            }

            protected override TableField GetKeyForItem(DataValue item) {
                return item.Field;
            }

            public void AddOrReplace(DataValue item) {
                if (Contains(item)) {
                    for (int i = 0; i < Count; i++) {
                        if (Items[i].Equals(item)) {
                            SetItem(i, item);
                        }
                    }
                } else {
                    Add(item);
                }
            }
        }
    }
}
