using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Enumerations;

namespace CloudyWing.MoneyTrack.Models.Queriers {
    /// <summary>
    /// Provides an abstract class for mapping data from a data source to a table.
    /// </summary>
    public abstract class MapTableQuerier : DataQuerier {
        private SqlConnection conn;
        private string orderBy;
        private DataFieldCollection fields;

        protected RecordModel RecordModel => RecordModel.Instance;

        protected CategoryModel CategoryModel => CategoryModel.Instance;

        /// <summary>
        /// Gets the connection to the database.
        /// </summary>
        protected SqlConnection Connection {
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
            private set => conn = value;
        }

        /// <summary>
        /// Gets or sets the command timeout for the query.
        /// </summary>
        public int CommanmdTimeout { get; set; } = 30;

        /// <summary>
        /// Gets the collection of fields to be included in the query.
        /// </summary>
        protected DataFieldCollection Fields {
            get {
                if (fields is null) {
                    fields = new DataFieldCollection();
                    InitFields();
                }
                return fields;
            }
        }

        /// <summary>
        /// Gets or sets the SQL column filter for the fields.
        /// </summary>
        public ISqlColumnFilter SqlColumnFilter {
            get => Fields.SqlColumnFilter;
            set => Fields.SqlColumnFilter = value;
        }

        /// <summary>
        /// Gets or sets the common table expression text for the query.
        /// </summary>
        public string CommonTableExpressionText { get; set; }

        /// <summary>
        /// Gets or sets the execution time for the query.
        /// </summary>
        public TimeSpan ExecutionTime { get; protected set; }

        /// <inheritdoc/>
        public override string OrderBy {
            get {
                if (orderBy is null) {
                    InitOrderBy();
                }
                return orderBy;
            }
            set {
                string orderBy = value;
                foreach (DataField field in Fields) {
                    string pattern0 = string.Format(@"^\s*{0}\s*$", field.DataKey);
                    string pattern1 = string.Format(@"^\s*{0}\s*", field.DataKey);
                    string pattern2 = string.Format(@"^\s*{0}\s*,\s*", field.DataKey);
                    string pattern3 = string.Format(@"\s*,\s{0}\s*", field.DataKey);
                    string pattern4 = string.Format(@"\s*,\s{0}\s*,\s*", field.DataKey);
                    string pattern5 = string.Format(@"\s*,\s{0}\s*$", field.DataKey);

                    // 取代掉單一排序條件
                    orderBy = Regex.Replace(orderBy, pattern0, field.SqlColumn, RegexOptions.IgnoreCase);
                    // 取代掉多排序條件
                    orderBy = Regex.Replace(orderBy, pattern1, field.SqlColumn + " ", RegexOptions.IgnoreCase);
                    orderBy = Regex.Replace(orderBy, pattern2, field.SqlColumn + ", ", RegexOptions.IgnoreCase);
                    orderBy = Regex.Replace(orderBy, pattern3, ", " + field.SqlColumn + " ", RegexOptions.IgnoreCase);
                    orderBy = Regex.Replace(orderBy, pattern4, ", " + field.SqlColumn + ", ", RegexOptions.IgnoreCase);
                    orderBy = Regex.Replace(orderBy, pattern5, ", " + field.SqlColumn, RegexOptions.IgnoreCase);
                }
                this.orderBy = orderBy;
            }
        }

        /// <summary>
        /// Gets the collection of data conditions used to filter the data returned by this provider.
        /// </summary>
        protected DataConditionCollection Conditions { get; } = new DataConditionCollection();

        /// <summary>
        /// Gets the SQL command used to retrieve data for the specified field.
        /// </summary>
        /// <param name="field">The field to retrieve data for.</param>
        /// <returns>A <see cref="SqlCommand"/> object used to retrieve data.</returns>
        protected abstract SqlCommand GetSqlCommand(string field);

        /// <summary>
        /// Gets the SQL command used to retrieve paged data.
        /// </summary>
        /// <returns>A <see cref="SqlCommand"/> object used to retrieve data.</returns>
        protected virtual SqlCommand GetPagedSqlCommand() {
            string top = string.Empty;
            int random = new Random().Next(100, 999);
            string result = string.Format("Result{0}", random);
            string rowNo = string.Format("RowNo{0}", random);
            int skip = (PageNumber - 1) * PageSize;
            int take = PageSize;

            if (take > 0) {
                top = string.Format("TOP {0}", skip + take);
            }
            SqlCommand cmd = GetSqlCommand($"{top} ROW_NUMBER() OVER (ORDER BY {OrderBy}) AS {rowNo}, {Fields} ");

            string sql = cmd.CommandText;

            cmd.CommandText = $@"
                SELECT * FROM (
                    {sql}
                ) {result}
                WHERE {result}.{rowNo} > {skip} ";

            return cmd;
        }

        /// <summary>
        /// Gets the SQL command used to retrieve data.
        /// </summary>
        /// <returns>A <see cref="SqlCommand"/> object used to retrieve data.</returns>
        protected SqlCommand GetSqlCommand(SqlConnection conn) {
            SqlCommand cmd;
            if ((PageNumber == 0 && PageSize == 0) || string.IsNullOrEmpty(OrderBy)) {
                cmd = GetSqlCommand(Fields.ToString());
                if (!string.IsNullOrEmpty(CommonTableExpressionText)) {
                    cmd.CommandText = CommonTableExpressionText + cmd.CommandText;
                }
                if (!string.IsNullOrEmpty(OrderBy)) {
                    cmd.CommandText += string.Format(" ORDER BY {0}", OrderBy);
                }

            } else {
                cmd = GetPagedSqlCommand();
                if (!string.IsNullOrEmpty(CommonTableExpressionText)) {
                    cmd.CommandText = CommonTableExpressionText + cmd.CommandText;
                }
            }
            cmd.Connection = conn;
            cmd.CommandTimeout = CommanmdTimeout;
            return cmd;
        }

        /// <summary>
        /// Initializes the list of fields to retrieve from the database.
        /// </summary>
        protected abstract void InitFields();

        /// <summary>
        /// Initializes the order by clause used to sort the results.
        /// </summary>
        protected virtual void InitOrderBy() {
            OrderBy = string.Empty;
        }

        /// <inheritdoc/>
        public override bool HasData() {
            return GetCount() > 0;
        }

        /// <inheritdoc/>
        public override int GetCount() {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try {
                using (SqlConnection conn = Connection)
                using (SqlCommand cmd = GetSqlCommand(" COUNT(1) ")) {
                    if (!string.IsNullOrEmpty(CommonTableExpressionText)) {
                        cmd.CommandText = CommonTableExpressionText + cmd.CommandText;
                    }
                    cmd.Connection = conn;
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count;
                }
            } finally {
                sw.Stop();
                ExecutionTime = sw.Elapsed;
                Connection = null;
            }
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row
        /// in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <returns>
        /// An <see cref="object"/> that is the first column of the first row
        /// in the result set. If the result set is empty, returns a null reference.
        /// </returns>
        public object GetDataScalar() {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try {
                using (SqlConnection conn = Connection)
                using (SqlCommand cmd = GetSqlCommand(conn)) {
                    object o = cmd.ExecuteScalar();

                    return o;
                }
            } finally {
                sw.Stop();
                ExecutionTime = sw.Elapsed;
                Connection = null;
            }
        }

        /// <summary>
        /// Sends the command to the connection and builds a <see cref="SqlDataReader"/>
        /// object using one of the <see cref="CommandBehavior"/> values.
        /// </summary>
        /// <param name="behavior">
        /// One of the <see cref="CommandBehavior"/> values.
        /// </param>
        /// <returns>
        /// A <see cref="SqlDataReader"/> object.
        /// </returns>
        public SqlDataReader GetDataReader(CommandBehavior behavior = CommandBehavior.SequentialAccess) {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try {
                SqlConnection conn = Connection;
                SqlCommand cmd = GetSqlCommand(conn);
                cmd.Connection = conn;
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection | behavior);

                return dr;
            } finally {
                sw.Stop();
                ExecutionTime = sw.Elapsed;
                Connection = null;
            }
        }

        /// <inheritdoc/>
        public override DataTable GetDataTable() {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try {
                using (SqlConnection conn = Connection)
                using (SqlCommand cmd = GetSqlCommand(conn)) {
                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    return dt;
                }
            } finally {
                sw.Stop();
                ExecutionTime = sw.Elapsed;
                Connection = null;
            }
        }

        /// <summary>
        /// Gets the paging metadata.
        /// </summary>
        /// <returns>
        /// A <see cref="PagingMetadata"/> object.
        /// </returns>
        public PagingMetadata GetPagingMetadata() {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try {
                return new PagingMetadata(PageNumber, PageSize, GetCount());
            } finally {
                sw.Stop();
                ExecutionTime = sw.Elapsed;
                Connection = null;
            }
        }

        /// <summary>
        /// Adds a condition to the data provider for table field, and array of values.
        /// </summary>
        /// <param name="prefix">The prefix of the condition.</param>
        /// <param name="field">The table field for the condition.</param>
        /// <param name="values">The array of values for the condition.</param>
        public void AddCondition(TableField field, params object[] values) {
            if (values is null) {
                return;
            }
            AddCondition(field, values.Select(x => x));
        }

        /// <summary>
        /// Adds a condition to the data provider for table field, and array of values.
        /// </summary>
        /// <param name="field">The table field for the condition.</param>
        /// <param name="values">The IEnumerable of values for the condition.</param>
        public void AddCondition(TableField field, IEnumerable<object> values) {
            if (values is null) {
                return;
            }
            AddCondition(field, CompareMode.Equal, values);
        }

        /// <summary>
        /// Adds a condition to the data provider for a specified prefix, table field, and array of values.
        /// </summary>
        /// <param name="prefix">The prefix of the condition.</param>
        /// <param name="field">The table field for the condition.</param>
        /// <param name="values">The array of values for the condition.</param>
        public void AddCondition(string prefix, TableField field, params object[] values) {
            if (values is null) {
                return;
            }
            AddCondition(prefix, field, CompareMode.Equal, values.Select(x => x));
        }

        /// <summary>
        /// Adds a condition to the data provider for a specified prefix, table field, and array of values.
        /// </summary>
        /// <param name="prefix">The prefix of the condition.</param>
        /// <param name="field">The table field for the condition.</param>
        /// <param name="values">The IEnumerable of values for the condition.</param>
        public void AddCondition(string prefix, TableField field, IEnumerable<object> values) {
            if (values is null) {
                return;
            }
            AddCondition(prefix, field, CompareMode.Equal, values);
        }

        /// <summary>
        /// Adds a condition to the data provider for a specified table field, comparison mode, and array of values.
        /// </summary>
        /// <param name="field">The table field for the condition.</param>
        /// <param name="mode">The comparison mode for the condition.</param>
        /// <param name="values">The array of values for the condition.</param>
        public void AddCondition(TableField field, CompareMode mode, params object[] values) {
            if (values is null) {
                return;
            }
            AddCondition(field, mode, values.Select(x => x));
        }

        /// <summary>
        /// Adds a condition to the data provider for a specified table field, comparison mode, and array of values.
        /// </summary>
        /// <param name="field">The table field for the condition.</param>
        /// <param name="mode">The comparison mode for the condition.</param>
        /// <param name="values">The IEnumerable of values for the condition.</param>
        public void AddCondition(TableField field, CompareMode mode, IEnumerable<object> values) {
            if (values is null) {
                return;
            }
            AddCondition("", field, mode, values);
        }

        /// <summary>
        /// Adds a condition to the data provider for a specified prefix, table field, comparison mode, and array of values.
        /// </summary>
        /// <param name="prefix">The prefix of the condition.</param>
        /// <param name="field">The table field for the condition.</param>
        /// <param name="mode">The comparison mode for the condition.</param>
        /// <param name="values">The array of values for the condition.</param>
        public void AddCondition(string prefix, TableField field, CompareMode mode, params object[] values) {
            if (values is null) {
                return;
            }
            AddCondition(prefix, field, mode, values.Select(x => x));
        }

        /// <summary>
        /// Adds a condition to the data provider for a specified prefix, table field, comparison mode, and array of values.
        /// </summary>
        /// <param name="prefix">The prefix of the condition.</param>
        /// <param name="field">The table field for the condition.</param>
        /// <param name="mode">The comparison mode for the condition.</param>
        /// <param name="values">The IEnumerable of values for the condition.</param>
        public virtual void AddCondition(string prefix, TableField field, CompareMode mode, IEnumerable<object> values) {
            if (values is null) {
                return;
            }
            values = values.Distinct().Select(x => x);
            int oldCount = values.Count();
            values = values.Where(x => x != null);
            int newCount = values.Count();

            // IEnumerable<object>{null}不列入正常條件，IEnumerable<object>{}可列入
            if (newCount > 0 || oldCount == 0) {
                DataCondition dc = new DataCondition(field, mode, prefix);
                Conditions.AddValue(dc, values);
            }
        }

        /// <summary>
        /// Provides functionality to create basic WHERE clauses for SQL queries based on the specified DataModel and DataConditionCollection.
        /// </summary>
        /// <param name="model">The DataModel to create the WHERE clauses from.</param>
        /// <param name="wheres">The SqlWhereHelper to add the WHERE clauses to.</param>
        /// <param name="cmd">The SqlCommand to add the parameters to.</param>
        /// <param name="conditions">The DataConditionCollection containing the conditions to create the WHERE clauses from.</param>
        /// <param name="aliases">The table alias to use in the column names.</param>
        /// <param name="prefix">The prefix to use for the parameter names.</param>
        protected void CreateBasicWhere(DataModel model, SqlWhereHelper wheres, SqlCommand cmd, DataConditionCollection conditions, string aliases = "", string prefix = "") {
            IEnumerable<CompareMode> modes = CompareMode.GetAll();

            foreach (TableField field in model.TableFields) {
                string column = (string.IsNullOrEmpty(aliases) ? "" : aliases + ".") + field.NameWithBrackets;
                string oriName = field.Name;
                string oriKey = (string.IsNullOrEmpty(prefix) ? "" : prefix + "_") + oriName;
                int random = new Random().Next(100, 999);

                foreach (CompareMode mode in modes) {
                    string key = mode == CompareMode.Equal ?
                        "@" + oriKey :
                        string.Format(
                            "@{0}{1}_{2}",
                            mode.Prefix, random, oriKey
                        );
                    if (conditions.TryFindDataCondition(field, mode, prefix, out DataCondition dc)) {
                        DatabaseUtils.AddWhere(
                            column, key, field, wheres, cmd, dc.Value, mode
                        );
                    }

                }
            }
        }

        /// <summary>
        /// Clears the conditions, initializes the fields, and initializes the order by clause.
        /// </summary>
        public virtual void ReSet() {
            Conditions.Clear();
            InitFields();
            InitOrderBy();
        }

        /// <summary>
        /// Checks that the specified TableField belongs to the specified DataModel.
        /// </summary>
        /// <param name="model">The DataModel to check.</param>
        /// <param name="field">The TableField to check.</param>
        /// <exception cref="ArgumentException">Thrown if the TableField does not belong to the specified DataModel.</exception>
        protected void CheckModel(DataModel model, TableField field) {
            if (field.Model != model) {
                throw new ArgumentException("TableField的DataModel錯誤，請輸入" + model.GetType() + "的TableField");
            }
        }

        /// <summary>
        /// Represents a condition applied to a table field.
        /// </summary>
        protected struct DataCondition {
            /// <summary>
            /// Initializes a new instance of the <see cref="DataCondition"/> struct.
            /// </summary>
            /// <param name="field">The <see cref="TableField"/> to which the condition is applied.</param>
            /// <param name="mode">The <see cref="CompareMode"/> of the condition.</param>
            /// <param name="prefix">The prefix used in the parameter name of the SQL command.</param>
            public DataCondition(TableField field, CompareMode mode, string prefix)
                : this() {
                Field = field;
                Mode = mode;
                Prefix = prefix;
            }

            /// <summary>
            /// Gets or sets the <see cref="TableField"/> to which the condition is applied.
            /// </summary>
            public TableField Field { get; set; }

            /// <summary>
            /// Gets or sets the prefix used in the parameter name of the SQL command.
            /// </summary>
            public string Prefix { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="CompareMode"/> of the condition.
            /// </summary>
            public CompareMode Mode { get; set; }

            /// <summary>
            /// Gets or sets the value(s) of the condition.
            /// </summary>
            public IList<object> Value { get; set; }

            /// <summary>
            /// Determines whether two <see cref="DataCondition"/> objects are equal.
            /// </summary>
            /// <param name="a">The first <see cref="DataCondition"/> to compare.</param>
            /// <param name="b">The second <see cref="DataCondition"/> to compare.</param>
            /// <returns><c>true</c> if the two <see cref="DataCondition"/> objects are equal; otherwise, <c>false</c>.</returns>
            public static bool operator ==(DataCondition a, DataCondition b) {
                return a.Field.Equals(b.Field) && a.Mode == b.Mode && a.Prefix == b.Prefix;
            }

            /// <summary>
            /// Determines whether two <see cref="DataCondition"/> objects are not equal.
            /// </summary>
            /// <param name="a">The first <see cref="DataCondition"/> to compare.</param>
            /// <param name="b">The second <see cref="DataCondition"/> to compare.</param>
            /// <returns><c>true</c> if the two <see cref="DataCondition"/> objects are not equal; otherwise, <c>false</c>.</returns>
            public static bool operator !=(DataCondition a, DataCondition b) {
                return !a.Field.Equals(b.Field) || (a.Mode != b.Mode && a.Prefix != b.Prefix);
            }

            // <summary>
            /// Determines whether the current <see cref="DataCondition"/> object is equal to another object.
            /// </summary>
            /// <param name="condition">The <see cref="DataCondition"/> object to compare with the current object.</param>
            /// <returns><c>true</c> if the current <see cref="DataCondition"/> object is equal to the <paramref name="condition"/> parameter; otherwise, <c>false</c>.</returns>
            public bool Equals(DataCondition condition) {
                return this == condition;
            }

            /// <summary>
            /// Determines whether this instance and another specified DataCondition object have the same value.
            /// </summary>
            /// <param name="condition">The DataCondition object to compare with this instance.</param>
            /// <returns>true if the value of the condition parameter is the same as the value of this instance; otherwise, false.</returns>
            public override bool Equals(object obj) {
                return obj is DataCondition condition && Equals(condition);
            }

            /// <summary>
            /// Serves as the default hash function.
            /// </summary>
            /// <returns>A hash code for the current DataCondition object.</returns>
            public override int GetHashCode() {
                return base.GetHashCode();
            }
        }

        /// <summary>
        /// Represents a collection of data conditions used for filtering data.
        /// </summary>
        protected class DataConditionCollection : Collection<DataCondition> {
            /// <summary>
            /// Gets or sets the DataCondition object at the specified index that matches the specified condition.
            /// </summary>
            /// <param name="condition">The condition to match.</param>
            /// <returns>The DataCondition object that matches the specified condition.</returns>
            public DataCondition this[DataCondition condition] {
                get => this[IndexOf(condition)];
                set => this[IndexOf(condition)] = value;
            }

            /// <summary>
            /// Adds a value to the DataCondition object that matches the specified condition.
            /// </summary>
            /// <param name="condition">The condition to match.</param>
            /// <param name="value">The value to add.</param>
            public void AddValue(DataCondition condition, IEnumerable<object> value) {
                if (Contains(condition)) {
                    DataCondition dc = this[condition];
                    foreach (object o in value) {
                        if (!dc.Value.Contains(o)) {
                            dc.Value.Add(o);
                        }
                    }
                    this[condition] = dc;
                } else {
                    condition.Value = value.ToList();
                    Add(condition);
                }
            }

            /// <summary>
            /// Finds the DataCondition object that matches the specified field, mode, and prefix.
            /// </summary>
            /// <param name="field">The TableField object to match.</param>
            /// <param name="mode">The CompareMode object to match.</param>
            /// <param name="prefix">The prefix to match.</param>
            /// <returns>The DataCondition object that matches the specified field, mode, and prefix.</returns>
            /// <exception cref="ArgumentException">Thrown when the condition does not exist.</exception>
            public DataCondition FindDataCondition(TableField field, CompareMode mode, string prefix) {
                return TryFindDataCondition(field, mode, prefix, out DataCondition dc) ? dc : throw new ArgumentException("項目不存在");
            }

            /// <summary>
            /// Tries to find the data condition that matches the specified table field, compare mode, and prefix.
            /// </summary>
            /// <param name="field">The table field to match.</param>
            /// <param name="mode">The compare mode to match.</param>
            /// <param name="prefix">The prefix to match.</param>
            /// <param name="condition">
            /// When this method returns, contains the data condition that matches the specified criteria, if found; otherwise, the default value for <see cref="DataCondition"/>.
            /// </param>
            /// <returns><c>true</c> if the data condition that matches the specified criteria is found; otherwise, <c>false</c>.</returns>
            public bool TryFindDataCondition(TableField field, CompareMode mode, string prefix, out DataCondition condition) {
                foreach (DataCondition dc in Items) {
                    bool isSameField = dc.Field.Equals(field);
                    bool isSameMode = dc.Mode == mode;

                    string prefixA = dc.Prefix ?? string.Empty;
                    string prefixB = prefix ?? string.Empty;
                    bool isSamePrefix = string.Compare(prefixA, prefixB, true) == 0;

                    if (isSameField && isSameMode && isSamePrefix) {
                        condition = dc;

                        return true;
                    }
                }
                condition = default;
                return false;
            }
        }
    }
}
