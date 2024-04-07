using System;
using System.Collections.Generic;
using System.Data;

namespace CloudyWing.MoneyTrack.Models.Queriers {
    /// <summary>
    /// Provides a cache-based implementation of <see cref="DataQuerier"/> that stores data in a <see cref="DataTable"/>.
    /// </summary>
    public class CacheDataQuerier : DataQuerier {
        private readonly DataTable dataTable;
        private readonly List<string> primaryKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDataQuerier"/> class.
        /// </summary>
        public CacheDataQuerier() : base() {
            dataTable = new DataTable {
                TableName = "CacheDataProvider"
            };
            primaryKeys = new List<string>();
            IsCached = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data is cached.
        /// </summary>
        public bool IsCached { get; set; }

        /// <summary>
        /// Initializes the cached data.
        /// </summary>
        protected virtual void InitData() {
            IsCached = true;
        }

        /// <summary>
        /// Determines whether the data provider has data.
        /// </summary>
        /// <returns>true if the data provider has data; otherwise, false.</returns>
        public override bool HasData() {
            if (!IsCached) {
                InitData();
            }
            return GetCount() > 0;
        }

        /// <summary>
        /// Gets the number of rows in the data provider.
        /// </summary>
        /// <returns>The number of rows in the data provider.</returns>
        public override int GetCount() {
            if (!IsCached) {
                InitData();
            }

            return dataTable.Rows.Count;
        }

        /// <summary>
        /// Gets the DataTable object containing the data.
        /// </summary>
        /// <returns>The DataTable object containing the data.</returns>
        public override DataTable GetDataTable() {
            if (!IsCached) {
                InitData();
            }

            int count = GetCount();
            bool hasData = HasData();

            int skip = (PageNumber - 1) * PageSize;
            int take = PageSize;

            if (hasData && skip <= count && take <= count) {
                DataTable source = dataTable.Clone();
                DataRow[] rows = dataTable.Select("", OrderBy);
                int end = (take > 0 && skip + take < count) ? skip + take : count;
                for (int i = skip; i < end; i++) {
                    source.ImportRow(rows[i]);
                }
                return source;
            } else {
                return dataTable.Copy();
            }
        }

        /// <summary>
        /// Gets a copy of the DataTable object containing the data.
        /// </summary>
        /// <returns>A copy of the DataTable object containing the data.</returns>
        public DataTable GetBasicDataTable() {
            if (!IsCached) {
                InitData();
            }

            return dataTable.Copy();
        }

        /// <summary>
        /// Sets the specified value in the DataTable object.
        /// </summary>
        /// <param name="pk">The primary key.</param>
        /// <param name="key">The column key.</param>
        /// <param name="value">The value to set.</param>
        public void SetData(int pk, string key, object value) {
            SetData(pk.ToString(), key, value);
        }

        /// <summary>
        /// Sets the specified value in the DataTable object.
        /// </summary>
        /// <param name="pk">The primary key.</param>
        /// <param name="key">The column key.</param>
        /// <param name="value">The value to set.</param>
        public void SetData(string pk, string key, object value) {
            AddColumn(key, value == DBNull.Value ? typeof(object) : value.GetType());
            if (!primaryKeys.Contains(pk)) {
                primaryKeys.Add(pk);
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);
            }
            dataTable.Rows[primaryKeys.IndexOf(pk)][key] = value;
        }

        /// <summary>
        /// Increases the value of a specified data column associated with the specified primary key by one.
        /// </summary>
        /// <param name="pk">The primary key associated with the data column.</param>
        /// <param name="key">The name of the data column.</param>
        public void CountData(int pk, string key) {
            CountData(pk.ToString(), key, 1m);
        }

        /// <summary>
        /// Increases the value of a specified data column associated with the specified primary key by one.
        /// </summary>
        /// <param name="pk">The primary key associated with the data column.</param>
        /// <param name="key">The name of the data column.</param>
        public void CountData(string pk, string key) {
            CountData(pk, key, 1m);
        }

        /// <summary>
        /// Increases the value of a specified data column associated with the specified primary key by the specified value.
        /// </summary>
        /// <param name="pk">The primary key associated with the data column.</param>
        /// <param name="key">The name of the data column.</param>
        /// <param name="value">The value to be added to the data column.</param>
        public void CountData(int pk, string key, object value) {
            CountData(pk, key, Convert.ToDecimal(value));
        }

        /// <summary>
        /// Increases the value of a specified data column associated with the specified primary key by the specified value.
        /// </summary>
        /// <param name="pk">The primary key associated with the data column.</param>
        /// <param name="key">The name of the data column.</param>
        /// <param name="value">The value to be added to the data column.</param>
        public void CountData(int pk, string key, decimal value) {
            CountData(pk.ToString(), key, value);
        }

        /// <summary>
        /// Increases the value of a specified data column associated with the specified primary key by the specified value.
        /// </summary>
        /// <param name="pk">The primary key associated with the data column.</param>
        /// <param name="key">The name of the data column.</param>
        /// <param name="value">The value to be added to the data column.</param>
        public void CountData(string pk, string key, object value) {
            CountData(pk, key, Convert.ToDecimal(value));
        }

        /// <summary>
        /// Increases the value of a specified data column associated with the specified primary key by the specified value.
        /// </summary>
        /// <param name="pk">The primary key associated with the data column.</param>
        /// <param name="key">The name of the data column.</param>
        /// <param name="value">The value to be added to the data column.</param>
        public void CountData(string pk, string key, decimal value) {
            AddColumn(key, typeof(decimal));
            if (!primaryKeys.Contains(pk)) {
                primaryKeys.Add(pk);
                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);
            }
            object count = dataTable.Rows[primaryKeys.IndexOf(pk)][key];
            if (count is null || count == DBNull.Value) {
                count = 0;
            }
            dataTable.Rows[primaryKeys.IndexOf(pk)][key] = Convert.ToDecimal(count) + value;
        }

        /// <summary>
        /// Clears all columns in the underlying DataTable.
        /// </summary>
        public void ClearColumns() {
            dataTable.Columns.Clear();
        }

        /// <summary>
        /// Adds a new column to the underlying DataTable with the specified key, type, and caption.
        /// </summary>
        /// <param name="key">The name of the column to add.</param>
        /// <param name="type">The data type of the column to add.</param>
        /// <param name="caption">The caption of the column to add.</param>
        public void AddColumn(string key, Type type) {
            AddColumn(key, type, key);
        }

        /// <summary>
        /// Adds a new column to the underlying DataTable with the specified key and type, and uses the key as the caption.
        /// </summary>
        /// <param name="key">The name of the column to add.</param>
        /// <param name="type">The data type of the column to add.</param>
        public void AddColumn(string key, Type type, string caption) {
            if (!dataTable.Columns.Contains(key)) {
                DataColumn column = new DataColumn {
                    ColumnName = key,
                    DataType = type,
                    Caption = caption
                };
                switch (type.Name) {
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "Single":
                    case "Double":
                    case "Decimal":
                        column.DefaultValue = 0;
                        break;
                }
                dataTable.Columns.Add(column);
            }
        }

        /// <summary>
        /// Determines whether the underlying DataTable contains a column with the specified key.
        /// </summary>
        /// <param name="key">The name of the column to check.</param>
        /// <returns>true if the column exists; otherwise, false.</returns>
        public bool HasColumn(string key) {
            return dataTable.Columns.Contains(key);
        }

        /// <summary>
        /// Gets a read-only list of all primary keys in the underlying DataTable.
        /// </summary>
        /// <returns>A read-only list of all primary keys in the DataTable.</returns>
        public IList<string> GetAllPrimaryKeys() {
            return primaryKeys.AsReadOnly();
        }

        /// <summary>
        /// Gets the index of the specified primary key in the list of all primary keys in the underlying DataTable.
        /// </summary>
        /// <param name="pk">The primary key to search for.</param>
        /// <returns>The zero-based index of the primary key, or -1 if it is not found.</returns>
        public int GetPrimaryKeyIndex(string pk) {
            return primaryKeys.IndexOf(pk);
        }

        /// <summary>
        /// Gets the value of the specified column in the row with the specified primary key in the underlying DataTable.
        /// </summary>
        /// <param name="pk">The primary key of the row to retrieve the value from.</param>
        /// <param name="key">The name of the column to retrieve the value from.</param>
        /// <returns>The value of the specified column in the specified row, or null if the row or column does not exist.</returns>
        public object GetValue(string pk, string key) {
            return dataTable.Rows[primaryKeys.IndexOf(pk)][key];
        }

        /// <summary>
        /// Gets the value of the specified column at the specified row index.
        /// </summary>
        /// <param name="index">The index of the row to get the value from.</param>
        /// <param name="key">The name of the column to get the value from.</param>
        /// <returns>The value of the specified column at the specified row index.</returns>
        public object GetValue(int index, string key) {
            return dataTable.Rows[index][key];
        }

        /// <summary>
        /// Determines whether the cache contains the specified primary key.
        /// </summary>
        /// <param name="pk">The primary key to check for.</param>
        /// <returns>true if the cache contains the specified primary key; otherwise, false.</returns>
        public bool ContainsPrimaryKey(string pk) {
            return primaryKeys.Contains(pk);
        }
    }
}
