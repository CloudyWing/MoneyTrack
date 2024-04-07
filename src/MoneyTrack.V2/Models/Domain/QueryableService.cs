using System;
using System.Collections.Generic;
using System.Linq;
using CloudyWing.MoneyTrack.Models.DataAccess;
using Dapper;

namespace CloudyWing.MoneyTrack.Models.Domain {
    /// <summary>
    /// Queryable Service base.
    /// Implement a basic query interface.
    /// </summary>
    /// <typeparam name="TCondition"></typeparam>
    /// <typeparam name="TRecord"></typeparam>
    public abstract class QueryableService<TCondition, TRecord> : DomainService
        where TCondition : class, new() {
        private const int DefaultCommandTimeout = 30;

        protected QueryableService(UnitOfWorker unitOfWorker) : base(unitOfWorker) { }

        protected abstract string DefaultColumnNamesOfQuery { get; }

        protected abstract string DefaultOrderBy { get; }

        protected virtual string CommonTableExpression => "";

        /// <summary>
        /// 額外調整 SQL 查詢出來的資料。
        /// </summary>
        protected virtual Func<TRecord, TRecord> ExtraDataSelector => null;

        public int GetCount(TCondition condition = null) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "COUNT(1)", out string sql, out DynamicParameters parameters);
            return UnitOfWorker.DbContext.DbConnection.ExecuteScalar<int>(
                $"{CommonTableExpression} {sql}", parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public bool QueryExists(TCondition condition = null) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "NULL AS [EMPTY]", out string sql, out DynamicParameters parameters);
            return UnitOfWorker.DbContext.DbConnection.ExecuteScalar<int>(
                $"{CommonTableExpression} SELECT (CASE WHEN EXISTS({sql}) THEN 1 ELSE 0 END) AS [value]",
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            ) == 1;
        }

        public TRecord GetSingle(TCondition condition = null, int commandTimeout = DefaultCommandTimeout) {
            condition = condition ?? new TCondition();

            TRecord record = GetSingle<TRecord>(condition, commandTimeout);
            return ExtraDataSelector is null ? record : ExtraDataSelector(record);
        }

        public T GetSingle<T>(TCondition condition = null, int commandTimeout = DefaultCommandTimeout) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "TOP 2 " + DefaultColumnNamesOfQuery, out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.QuerySingle<T>(
                SqlUtils.EnsureRecompileOption($"{CommonTableExpression} {SqlUtils.RemoveNoLockHint(sql)}"),
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction,
                commandTimeout
            );
        }

        public TRecord GetSingleOrDefault(TCondition condition = null, int commandTimeout = DefaultCommandTimeout) {
            condition = condition ?? new TCondition();

            TRecord record = GetSingleOrDefault<TRecord>(condition, commandTimeout);

            return (record == null || ExtraDataSelector is null) ? record : ExtraDataSelector(record);
        }

        public T GetSingleOrDefault<T>(TCondition condition = null, int commandTimeout = DefaultCommandTimeout) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "TOP 2 " + DefaultColumnNamesOfQuery, out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.QuerySingleOrDefault<T>(
                SqlUtils.EnsureRecompileOption($"{CommonTableExpression} {SqlUtils.RemoveNoLockHint(sql)}"),
                parameters, commandTimeout: commandTimeout
            );
        }

        public TRecord GetFirst(TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout) {
            condition = condition ?? new TCondition();

            TRecord record = GetFirst<TRecord>(condition, orderBy, commandTimeout);

            return ExtraDataSelector is null ? record : ExtraDataSelector(record);
        }

        public T GetFirst<T>(TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "TOP 1 " + DefaultColumnNamesOfQuery, out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.QueryFirst<T>(
                SqlUtils.EnsureRecompileOption($"{CommonTableExpression} {SqlUtils.RemoveNoLockHint(sql)} ORDER BY {FixOrderBy(orderBy)}"),
                parameters, commandTimeout: commandTimeout
            );
        }

        public TRecord GetFirstOrDefault(TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout) {
            condition = condition ?? new TCondition();

            TRecord record = GetFirstOrDefault<TRecord>(condition, orderBy, commandTimeout);

            return (record == null || ExtraDataSelector is null) ? record : ExtraDataSelector(record);
        }

        public T GetFirstOrDefault<T>(TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "TOP 1 " + DefaultColumnNamesOfQuery, out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.QueryFirstOrDefault<T>(
                SqlUtils.EnsureRecompileOption($"{CommonTableExpression} {SqlUtils.RemoveNoLockHint(sql)} ORDER BY {FixOrderBy(orderBy)}"),
                parameters, commandTimeout: commandTimeout
            );
        }

        public IEnumerable<TRecord> GetList(
            TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout
        ) {
            IEnumerable<TRecord> list = GetList<TRecord>(DefaultColumnNamesOfQuery, condition, orderBy, commandTimeout);

            return ExtraDataSelector is null
                ? list : list.Select(ExtraDataSelector);
        }

        public IEnumerable<T> GetList<T>(
            TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout
        ) {
            return GetList<T>(null, condition, orderBy, commandTimeout);
        }

        public IEnumerable<T> GetList<T>(
            string columnNames, TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout
        ) {
            columnNames = columnNames ?? DefaultColumnNamesOfQuery;
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, columnNames, out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.Query<T>(
                SqlUtils.EnsureRecompileOption($"{CommonTableExpression} {sql} ORDER BY {FixOrderBy(orderBy)}"),
                parameters, commandTimeout: commandTimeout
            );
        }

        public PagedList<TRecord> GetPagedList(
            int pageNumber, int pageSize, TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout
        ) {
            PagedList<TRecord> list = GetPagedList<TRecord>(pageNumber, pageSize, condition, orderBy, commandTimeout);

            return ExtraDataSelector is null
                ? list
                : new PagedList<TRecord>(list.Select(ExtraDataSelector), list);
        }

        public PagedList<T> GetPagedList<T>(
            int pageNumber, int pageSize, TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout
        ) {
            return GetPagedList<T>(pageNumber, pageSize, DefaultColumnNamesOfQuery, condition, orderBy, commandTimeout);
        }

        public PagedList<T> GetPagedList<T>(
            int pageNumber, int pageSize, string columnNames,
            TCondition condition = null, string orderBy = null, int commandTimeout = DefaultCommandTimeout
        ) {
            string top = $"TOP {SqlUtils.GetLastItemOnPage(pageNumber, pageSize)}";
            int random = new Random().Next(100, 999);
            string result = $"Result{random}";
            string rowNo = $"RowNo{random}";
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(
                condition,
                $"{top} ROW_NUMBER() OVER (ORDER BY {FixOrderBy(orderBy)}) AS {rowNo}, {columnNames} ",
                out string sql, out DynamicParameters parameters
            );

            sql = $@"
                {CommonTableExpression}
                SELECT * FROM (
                    {sql}
                ) {result}
                WHERE {result}.{rowNo} >= {SqlUtils.GetFirstItemOnPage(pageNumber, pageSize)} ";

            int totalItemCount = GetCount(condition);

            IEnumerable<T> list = UnitOfWorker.DbContext.DbConnection.Query<T>(SqlUtils.EnsureRecompileOption(sql), parameters, commandTimeout: commandTimeout);

            return new PagedList<T>(list, pageNumber, pageSize, totalItemCount);
        }

        private string FixOrderBy(string orderBy) {
            return SqlUtils.RemoveOrderByClause(orderBy ?? DefaultOrderBy);
        }

        protected abstract void CreateSqlInfoOfQuery(
            TCondition condition, string columnNames,
            out string resultSql, out DynamicParameters resultParameters
        );
    }
}
