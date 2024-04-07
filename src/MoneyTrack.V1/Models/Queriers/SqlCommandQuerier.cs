using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace CloudyWing.MoneyTrack.Models.Queriers {
    /// <summary>
    /// Provides a Provider for executing a SQL command.
    /// </summary>
    public sealed class SqlCommandQuerier : MapTableQuerier {
        private SqlCommand sqlCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommandQuerier"/> class with the specified SQL command text and common table expression (CTE).
        /// </summary>
        /// <param name="sql">The SQL command text to execute.</param>
        /// <param name="cte">The common table expression (CTE) to use with the command.</param>
        public SqlCommandQuerier(string sql, string cte = "") : this(new SqlCommand(sql), cte) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommandQuerier"/> class with the specified <see cref="SqlCommand"/> and common table expression (CTE).
        /// </summary>
        /// <param name="cmd">The <see cref="SqlCommand"/> to execute.</param>
        /// <param name="cte">The common table expression (CTE) to use with the command.</param>
        public SqlCommandQuerier(SqlCommand cmd, string cte = "") {
            SqlCommand = cmd;
            CommonTableExpressionText = cte;
        }

        /// <summary>
        /// Gets or sets the <see cref="SqlCommand"/> object used to execute the command.
        /// </summary>
        public SqlCommand SqlCommand {
            get => sqlCommand;
            set {
                CommandText = value.CommandText;
                sqlCommand = value;
            }
        }

        /// <summary>
        /// Gets or sets the command text to execute.
        /// </summary>
        public string CommandText { get; set; }

        /// <inheritdoc/>
        public override int GetCount() {
            if (string.IsNullOrEmpty(OrderBy)) {
                return GetDataTable().Rows.Count;
            } else {
                using (SqlCommand cmd = SqlCommand) {
                    cmd.CommandText = string.Format(
                        "{0} SELECT COUNT(1) FROM ({1}) Count{2}",
                        CommonTableExpressionText,
                        CommandText,
                        new Random().Next(100, 999)
                    );
                    cmd.Connection = Connection;
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count;
                }
            }
        }

        /// <inheritdoc/>
        protected override SqlCommand GetSqlCommand(string field) {
            SqlCommand.CommandText = CommandText;
            return SqlCommand;
        }

        /// <inheritdoc/>
        protected override SqlCommand GetPagedSqlCommand() {
            string top = string.Empty;
            int random = new Random().Next(100, 999);
            string basic = string.Format("Basic{0}", random);
            string result = string.Format("Result{0}", random);
            string rowNo = string.Format("RowNo{0}", random);
            string orderBy = OrderBy;
            if (orderBy.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) == -1) {
                orderBy = Regex.Replace(orderBy, @"\w+\.", basic + ".");
            }
            int skip = (PageNumber - 1) * PageSize;
            int take = PageSize;

            if (take > 0) {
                top = string.Format("TOP {0}", skip + take);
            }
            SqlCommand cmd = GetSqlCommand("");
            string sql = CommandText;
            cmd.CommandText = $@"
            SELECT * FROM (
                SELECT {top} ROW_NUMBER() OVER (ORDER BY {orderBy}) AS {rowNo}, * FROM (
                    {sql}
                ) {basic}
            ) {result}
            WHERE {result}.{rowNo} > {skip} ";

            return cmd;
        }

        /// <inheritdoc/>
        protected override void InitFields() { }

        /// <inheritdoc/>
        protected override void InitOrderBy() { }
    }
}
