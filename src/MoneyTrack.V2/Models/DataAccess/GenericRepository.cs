using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Dapper;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public class GenericRepository<TCondition, TEntity, TKey>
        where TCondition : class, new()
        where TEntity : class {
        public GenericRepository(UnitOfWorker unitOfWorker) {
            ExceptionUtils.ThrowIfNull(() => unitOfWorker);

            UnitOfWorker = unitOfWorker;
        }

        protected UnitOfWorker UnitOfWorker { get; }

        public void Add(TEntity entity) {
            ModificationCommand command = new ModificationCommand(
                ModificationType.Insert, () => {
                    AddAction(entity, UnitOfWorker.DbContext.CurrentTransaction);
                    return 1;
                });

            UnitOfWorker.AddModificationCommand(command);
        }

        protected virtual void AddAction(TEntity entity, DbTransaction transaction = null) {
            object id = UnitOfWorker.DbContext.DbConnection.Insert<TKey, TEntity>(
                entity, transaction
            );

            List<PropertyInfo> props = entity.GetType().GetProperties()
                .Where(x => x.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() != null)
                .ToList();

            if (props.Count == 1) {
                props.Single().SetValue(entity, id);
            }
        }

        public void Update(TEntity entity) {
            ModificationCommand command = new ModificationCommand(
                ModificationType.Update,
                () => UpdateAction(entity, UnitOfWorker.DbContext.CurrentTransaction)
            );

            UnitOfWorker.AddModificationCommand(command);
        }

        protected virtual int UpdateAction(TEntity entity, DbTransaction transaction = null) {
            return UnitOfWorker.DbContext.DbConnection.Update(
                entity, transaction
            );
        }

        public void Delete(TKey key) {
            ModificationCommand command = new ModificationCommand(
                ModificationType.Delete,
                () => DeleteAction(key, UnitOfWorker.DbContext.CurrentTransaction)
            );

            UnitOfWorker.AddModificationCommand(command);
        }

        protected virtual int DeleteAction(TKey key, DbTransaction transaction = null) {
            return UnitOfWorker.DbContext.DbConnection.Delete<TEntity>(
                key, transaction
            );
        }

        public int QueryCount(TCondition condition = null) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "COUNT(1)", out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.ExecuteScalar<int>(
                sql, parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public bool QueryExists(TKey key) {
            return UnitOfWorker.DbContext.DbConnection.Get<TEntity>(
                key, UnitOfWorker.DbContext.CurrentTransaction
            ) != null;
        }

        public bool QueryExists(TCondition condition = null) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "NULL AS [EMPTY]", out string sql, out DynamicParameters parameters);
            return UnitOfWorker.DbContext.DbConnection.ExecuteScalar<int>(
                $"SELECT (CASE WHEN EXISTS({sql}) THEN 1 ELSE 0 END) AS [value]",
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            ) == 1;
        }

        public TEntity QuerySingle(TCondition condition = null) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "TOP 2 *", out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.QuerySingle<TEntity>(
                SqlUtils.EnsureRecompileOption(sql), parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public TEntity QuerySingleOrDefault(TKey key) {
            return UnitOfWorker.DbContext.DbConnection.Get<TEntity>(
                key, UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public TEntity QuerySingleOrDefault(TCondition condition = null) {
            condition = condition ?? new TCondition();

            CreateSqlInfoOfQuery(condition, "TOP 2 *", out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.QuerySingleOrDefault<TEntity>(
                SqlUtils.EnsureRecompileOption(sql), parameters, UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public TEntity QueryFirst(TCondition condition = null, string orderBy = null) {
            condition = condition ?? new TCondition();
            orderBy = orderBy ?? SqlUtils.GetPrimaryKeyName<TEntity>();

            CreateSqlInfoOfQuery(condition, "TOP 1 *", out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.QueryFirst<TEntity>(
                SqlUtils.EnsureRecompileOption($"{sql} ORDER BY {SqlUtils.RemoveOrderByClause(orderBy)}"),
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public TEntity QueryFirstOrDefault(TCondition condition, string orderBy = null) {
            condition = condition ?? new TCondition();
            orderBy = orderBy ?? SqlUtils.GetPrimaryKeyName<TEntity>();

            CreateSqlInfoOfQuery(condition, "TOP 1 *", out string sql, out DynamicParameters parameters);

            return UnitOfWorker.DbContext.DbConnection.QueryFirstOrDefault<TEntity>(
                SqlUtils.EnsureRecompileOption($"{sql} ORDER BY {SqlUtils.RemoveOrderByClause(orderBy)}"),
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        public IEnumerable<TEntity> QueryList(TCondition condition = null, string orderBy = null) {
            condition = condition ?? new TCondition();
            orderBy = orderBy ?? SqlUtils.GetPrimaryKeyName<TEntity>();
            CreateSqlInfoOfQuery(condition, "*", out string sql, out DynamicParameters parameters);

            string fullSql = $"{sql} ORDER BY {SqlUtils.RemoveOrderByClause(orderBy)}";
            return ExecuteQuery(fullSql, parameters);
        }

        public PagedList<TEntity> QueryPagedList(int pageNumber, int pageSize, string orderBy = null) {
            return QueryPagedList(pageNumber, pageSize, new TCondition(), orderBy);
        }

        public PagedList<TEntity> QueryPagedList(
            int pageNumber, int pageSize, TCondition condition = null, string orderBy = null
        ) {
            string top = $"TOP {SqlUtils.GetLastItemOnPage(pageNumber, pageSize)}";
            int random = new Random().Next(100, 999);
            string result = $"Result{random}";
            string rowNo = $"RowNo{random}";
            condition = condition ?? new TCondition();
            orderBy = orderBy ?? SqlUtils.GetPrimaryKeyName<TEntity>();

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

            int totalItemCount = QueryCount(condition);

            IEnumerable<TEntity> list = ExecuteQuery(orderedSql, parameters);

            return new PagedList<TEntity>(list, pageNumber, pageSize, totalItemCount);
        }

        private IEnumerable<TEntity> ExecuteQuery(string sql, DynamicParameters parameters) {
            return UnitOfWorker.DbContext.DbConnection.Query<TEntity>(
                SqlUtils.EnsureRecompileOption(sql),
                parameters,
                UnitOfWorker.DbContext.CurrentTransaction
            );
        }

        private static void CreateSqlInfoOfQuery(TCondition condition, string columnNames, out string resultSql, out DynamicParameters resultParameters) {
            resultParameters = new DynamicParameters();
            WhereClauseBuilder clauseBuilder = new WhereClauseBuilder();
            ConditionParser<TCondition> parser = new ConditionParser<TCondition>(condition, clauseBuilder, resultParameters);

            Type conditionType = typeof(TCondition);

            foreach (PropertyInfo property in conditionType.GetProperties()) {
                string columnName = property.Name;
                Type conditionColumnType = property.PropertyType;

                if (conditionColumnType.IsGenericType && conditionColumnType.GetGenericTypeDefinition() == typeof(ConditionColumn<>)) {
                    Type valueType = conditionColumnType.GetGenericArguments()[0];
                    MethodInfo method = typeof(ConditionParser<>)
                        .MakeGenericType(conditionType)
                        .GetMethod("ParseColumnToWhereInfo")
                        .MakeGenericMethod(valueType);

                    var expr = CreateExpression(conditionType, property);
                    method.Invoke(parser, new object[] { expr, columnName });
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
