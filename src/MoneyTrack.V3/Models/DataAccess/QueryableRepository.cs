using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Dapper;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public abstract class QueryableRepository<TCondition, TEntity, TKey>
        where TCondition : class, new()
        where TEntity : class {
        protected QueryableRepository(UnitOfWorker unitOfWorker) {
            ExceptionUtils.ThrowIfNull(() => unitOfWorker);

            UnitOfWorker = unitOfWorker;
        }

        protected UnitOfWorker UnitOfWorker { get; }

        public async Task<bool> QueryExistsAsync(TKey key, DbTransaction? transaction = null) {
            return (await QueryAsync(key, transaction)) != null;
        }

        public async Task<TEntity> QueryAsync(TKey key, DbTransaction? transaction = null) {
            return await UnitOfWorker.DbContext.DbConnection.GetAsync<TEntity>(key, transaction);
        }

        public async Task<int> QueryCountAsync(TCondition? condition = null) {
            condition ??= new TCondition();
            CreateSqlInfoOfQuery(condition, "COUNT(1)", out string sql, out DynamicParameters parameters);

            return await UnitOfWorker.DbContext.DbConnection.ExecuteScalarAsync<int>(
                sql, parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public async Task<bool> QueryExistsAsync(TKey key) {
            return await UnitOfWorker.DbContext.DbConnection.GetAsync<TEntity>(
                key, UnitOfWorker.DbContext.CurrentTransaction
            ) is not null;
        }

        public async Task<bool> QueryExistsAsync(TCondition condition) {
            CreateSqlInfoOfQuery(condition, "NULL AS [EMPTY]", out string sql, out DynamicParameters parameters);
            return await UnitOfWorker.DbContext.DbConnection.ExecuteScalarAsync<int>(
                $"SELECT (CASE WHEN EXISTS({sql}) THEN 1 ELSE 0 END) AS [value]",
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            ) == 1;
        }

        public async Task<TEntity> QuerySingleAsync(TCondition condition) {
            CreateSqlInfoOfQuery(condition, "TOP 2 *", out string sql, out DynamicParameters parameters);

            return await UnitOfWorker.DbContext.DbConnection.QuerySingleAsync<TEntity>(
                SqlUtils.EnsureRecompileOption(sql), parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public async Task<TEntity?> QuerySingleOrDefaultAsync(TKey key) {
            return await UnitOfWorker.DbContext.DbConnection.GetAsync<TEntity>(
                key, UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public async Task<TEntity?> QuerySingleOrDefaultAsync(TCondition condition) {
            CreateSqlInfoOfQuery(condition, "TOP 2 *", out string sql, out DynamicParameters parameters);

            return await UnitOfWorker.DbContext.DbConnection.QuerySingleOrDefaultAsync<TEntity>(
                SqlUtils.EnsureRecompileOption(sql), parameters, UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public async Task<TEntity> QueryFirstAsync(TCondition? condition = null, string? orderBy = null) {
            condition ??= new TCondition();
            orderBy ??= SqlUtils.GetPrimaryKeyName<TEntity>();

            CreateSqlInfoOfQuery(condition, "TOP 1 *", out string sql, out DynamicParameters parameters);

            return await UnitOfWorker.DbContext.DbConnection.QueryFirstAsync<TEntity>(
                SqlUtils.EnsureRecompileOption($"{sql} ORDER BY {SqlUtils.RemoveOrderByClause(orderBy)}"),
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public async Task<TEntity?> QueryFirstOrDefaultAsync(TCondition? condition = null, string? orderBy = null) {
            condition ??= new TCondition();
            orderBy ??= SqlUtils.GetPrimaryKeyName<TEntity>();

            CreateSqlInfoOfQuery(condition, "TOP 1 *", out string sql, out DynamicParameters parameters);

            return await UnitOfWorker.DbContext.DbConnection.QueryFirstOrDefaultAsync<TEntity>(
                SqlUtils.EnsureRecompileOption($"{sql} ORDER BY {SqlUtils.RemoveOrderByClause(orderBy)}"),
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public async Task<IEnumerable<TEntity>> QueryListAsync(TCondition? condition = null, string? orderBy = null) {
            condition ??= new TCondition();
            orderBy ??= SqlUtils.GetPrimaryKeyName<TEntity>();
            CreateSqlInfoOfQuery(condition, "*", out string sql, out DynamicParameters parameters);

            string fullSql = $"{sql} ORDER BY {SqlUtils.RemoveOrderByClause(orderBy)}";
            return await ExecuteQueryAsync(fullSql, parameters);
        }

        public async Task<PagedList<TEntity>> QueryPagedListAsync(
            int pageNumber, int pageSize, TCondition? condition = null, string? orderBy = null
        ) {
            string top = $"TOP {SqlUtils.GetLastItemOnPage(pageNumber, pageSize)}";
            int random = new Random().Next(100, 999);
            string result = $"Result{random}";
            string rowNo = $"RowNo{random}";
            condition ??= new TCondition();
            orderBy ??= SqlUtils.GetPrimaryKeyName<TEntity>();

            CreateSqlInfoOfQuery(
                condition,
                $"{top} ROW_NUMBER() OVER (ORDER BY {SqlUtils.RemoveOrderByClause(orderBy)}) AS {rowNo}, * ",
                out string sql, out DynamicParameters parameters
            );

            string orderedSql = $@"
                SELECT * FROM (
                    {sql}
                ) {result}
                WHERE {result}.{rowNo} >= {SqlUtils.GetFirstItemOnPage(pageNumber, pageSize)} ";

            Task<int> totalItemCountTask = QueryCountAsync(condition);

            Task<IEnumerable<TEntity>> listTask = ExecuteQueryAsync(orderedSql, parameters);

            return new PagedList<TEntity>(await listTask, pageNumber, pageSize, await totalItemCountTask);
        }

        private async Task<IEnumerable<TEntity>> ExecuteQueryAsync(string sql, DynamicParameters parameters) {
            return await UnitOfWorker.DbContext.DbConnection.QueryAsync<TEntity>(
                SqlUtils.EnsureRecompileOption(sql),
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        private static void CreateSqlInfoOfQuery(TCondition condition, string columnNames, out string resultSql, out DynamicParameters resultParameters) {
            resultParameters = new DynamicParameters();
            WhereClauseBuilder clauseBuilder = new();
            ConditionParser<TCondition> parser = new(condition, clauseBuilder, resultParameters);

            Type conditionType = typeof(TCondition);

            foreach (PropertyInfo property in conditionType.GetProperties()) {
                string columnName = property.Name;
                Type conditionColumnType = property.PropertyType;

                if (conditionColumnType.IsGenericType && conditionColumnType.GetGenericTypeDefinition() == typeof(ConditionColumn<>)) {
                    Type valueType = conditionColumnType.GetGenericArguments()[0];
                    MethodInfo method = typeof(ConditionParser<>)
                        .MakeGenericType(conditionType)
                        .GetMethod("ParseColumnToWhereInfo")!
                        .MakeGenericMethod(valueType);

                    var expr = QueryableRepository<TCondition, TEntity, TKey>.CreateExpression(conditionType, property);
                    method.Invoke(parser, [expr, columnName]);
                }
            }

            resultSql = $@"
                SELECT {columnNames}
                FROM {SqlUtils.GetTableName<TEntity>()} WITH (NOLOCK)
                {clauseBuilder.StartWhereSeparatorAnd}";
        }

        private static dynamic CreateExpression(Type conditionType, PropertyInfo property) {
            ParameterExpression parameterExpr = Expression.Parameter(conditionType, "x");
            MemberExpression propertyExpr = Expression.Property(parameterExpr, property);
            Type funcType = typeof(Func<,>).MakeGenericType(conditionType, property.PropertyType);
            LambdaExpression lambda = Expression.Lambda(funcType, propertyExpr, parameterExpr);

            return lambda;
        }
    }
}
