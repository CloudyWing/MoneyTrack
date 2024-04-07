using System.Linq.Expressions;
using System.Reflection;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Models.Domain.Statements;
using Dapper;
using ParameterInfo = CloudyWing.MoneyTrack.Models.Domain.Statements.ParameterInfo;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public abstract class QueryableService<TEntity> : DomainService where TEntity : class, new() {
        private string? sqlBody;

        public QueryableService(UnitOfWorker? unitOfWorker, IServiceProvider? serviceProvider)
            : base(unitOfWorker, serviceProvider) { }

        public virtual string SqlBody {
            get {
                if (sqlBody is null) {
                    TableAttribute? attr = typeof(TEntity).GetCustomAttribute<TableAttribute>();
                    sqlBody = $"FROM {attr?.Name ?? typeof(TEntity).Name}";
                }

                return sqlBody;
            }
        }

        public virtual string QuerySqlPattern => $@"
SELECT {{SelectColumns}}
{SqlBody}
{{WhereClause}}
{{OrderByClause}}";

        public virtual string QueryPagedSqlPattern => $@"
SELECT * FROM (
    SELECT TOP (@PageNumber * @PageSize) ROW_NUMBER() OVER ({{OrderByClause}}) AS RowNum, {{SelectColumns}}
    {SqlBody}
    {{WhereClause}}
) AS u
WHERE u.RowNum > ((@PageNumber - 1) * @PageSize) ";

        public async Task<bool> IsExistsAsync(Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null, int? commandTimeout = null) {
            return await GetCountAsync(filter, commandTimeout) > 0;
        }

        public async Task<int> GetCountAsync(Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null, int? commandTimeout = null) {
            return await GetInternalAsync<int, string>(
                (sql, param) => UnitOfWorker.DbContext.DbConnection.ExecuteScalarAsync<int>(sql, param, commandTimeout: commandTimeout),
                x => x.Count(x => "RecordCount"), filter, null
            );
        }

        public async Task<TRecord> GetFirstAsync<TRecord>(
            Action<SelectProvider<TEntity, TRecord>> selector,
            Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null,
            Action<OrderProvider<TEntity>>? sorter = null,
            int? commandTimeout = null
        ) where TRecord : class {
            return await GetInternalAsync(
                (sql, param) => UnitOfWorker.DbContext.DbConnection.QueryFirstAsync<TRecord>(sql, param, commandTimeout: commandTimeout),
                selector, filter, sorter
            );
        }

        public async Task<TRecord?> GetFirstOrDefaultAsync<TRecord>(
            Action<SelectProvider<TEntity, TRecord>> selector,
            Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null,
            Action<OrderProvider<TEntity>>? sorter = null,
            int? commandTimeout = null
        ) where TRecord : class {
            return await GetInternalAsync(
                 (sql, param) => UnitOfWorker.DbContext.DbConnection.QueryFirstOrDefaultAsync<TRecord>(sql, param, commandTimeout: commandTimeout),
                 selector, filter, sorter
            );
        }

        public async Task<TRecord> GetSingleAsync<TRecord>(
            Action<SelectProvider<TEntity, TRecord>> selector,
            Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null,
            Action<OrderProvider<TEntity>>? sorter = null,
            int? commandTimeout = null
        ) where TRecord : class {
            return await GetInternalAsync(
                (sql, param) => UnitOfWorker.DbContext.DbConnection.QuerySingleAsync<TRecord>(sql, param, commandTimeout: commandTimeout),
                selector, filter, sorter
            );
        }

        public async Task<TRecord?> GetSingleOrDefaultAsync<TRecord>(
            Action<SelectProvider<TEntity, TRecord>> selector,
            Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null,
            Action<OrderProvider<TEntity>>? sorter = null,
            int? commandTimeout = null
        ) where TRecord : class {
            return await GetInternalAsync(
                (sql, param) => UnitOfWorker.DbContext.DbConnection.QuerySingleOrDefaultAsync(sql, param, commandTimeout: commandTimeout),
                selector, filter, sorter
            );
        }

        public async Task<IEnumerable<TRecord>> GetListAsync<TRecord>(
            Action<SelectProvider<TEntity, TRecord>> selector,
            Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null,
            Action<OrderProvider<TEntity>>? sorter = null,
            int? commandTimeout = null
        ) where TRecord : class {
            return await GetInternalAsync(
                (sql, param) => UnitOfWorker.DbContext.DbConnection.QueryAsync<TRecord>(sql, param, commandTimeout: commandTimeout),
                selector, filter, sorter
            );
        }

        private async Task<TResult> GetInternalAsync<TResult, TRecord>(
            Func<string, DynamicParameters, Task<TResult>> func,
            Action<SelectProvider<TEntity, TRecord>>? selector = null,
            Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null,
            Action<OrderProvider<TEntity>>? sorter = null
         ) where TRecord : class {
            Specification<TEntity, TRecord> spec = new() {
                Selector = selector,
                Filter = filter,
                Sorter = sorter
            };

            string sql = CreateQuerySql(spec, out DynamicParameters parameters);

            return await func(sql, parameters);
        }

        protected string CreateQuerySql<TRecord>(Specification<TEntity, TRecord> specification, out DynamicParameters parameters)
            where TRecord : class {
            string selectColumns = "*";

            if (specification.Selector is not null) {
                SelectProvider<TEntity, TRecord> selectProvider = new();
                specification.Selector(selectProvider);

                selectColumns = selectProvider.ToString()!;
            }

            WhereClauseBuilder clauseBuilder = new();
            parameters = new();

            if (specification.Filter is not null) {
                FilterProvider<TEntity> filterProvider = new();
                IOperatorStatement operatorStatement = specification.Filter(filterProvider);

                if (operatorStatement is not null) {
                    clauseBuilder.AppendIfNotEmpty(operatorStatement.StatementString);

                    foreach (ParameterInfo pi in operatorStatement.Parameters) {
                        parameters.Add(pi.ParameterName, pi.Value);
                    }
                }
            }

            string orderByClause = "";
            if (specification.Sorter is not null) {
                OrderProvider<TEntity> orderProvider = new();
                specification.Sorter(orderProvider);
                orderByClause = orderProvider.ToString();
            }

            return QuerySqlPattern.Replace("{SelectColumns}", selectColumns)
                .Replace("{WhereClause}", clauseBuilder.StartWhereSeparatorAnd)
                .Replace("{OrderByClause}", orderByClause);
        }

        public async Task<PagedList<TRecord>> GetPagedList<TRecord>(
            Action<SelectProvider<TEntity, TRecord>> selector,
            Action<OrderProvider<TEntity>> sorter,
            int pageNumber, int pageSize,
            Func<FilterProvider<TEntity>, IOperatorStatement>? filter = null,
            int? commandTimeout = null
        ) where TRecord : class {
            int count = await GetCountAsync(filter);

            IEnumerable<TRecord> records = await GetPagedListInternalAsync(
                selector, sorter, pageNumber, pageSize, filter, commandTimeout
            );

            return new PagedList<TRecord>(records, pageNumber, pageSize, count);
        }

        private async Task<IEnumerable<TRecord>> GetPagedListInternalAsync<TRecord>(
            Action<SelectProvider<TEntity, TRecord>>? selector,
            Action<OrderProvider<TEntity>> sorter,
            int pageNumber, int pageSize,
            Func<FilterProvider<TEntity>, IOperatorStatement>? filter,
            int? commandTimeout
        ) where TRecord : class {
            Specification<TEntity, TRecord> spec = new() {
                Selector = selector,
                Filter = filter,
                Sorter = sorter,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            string sql = CreateQueryPagedSql(spec, out DynamicParameters parameters);

            return await UnitOfWorker.DbContext.DbConnection.QueryAsync<TRecord>(sql, parameters, commandTimeout: commandTimeout);
        }

        protected string CreateQueryPagedSql<TRecord>(Specification<TEntity, TRecord> specification, out DynamicParameters parameters)
            where TRecord : class {
            string selectColumns = "*";

            if (specification.Selector is not null) {
                SelectProvider<TEntity, TRecord> selectProvider = new();
                specification.Selector(selectProvider);

                selectColumns = selectProvider.ToString()!;
            }

            WhereClauseBuilder clauseBuilder = new();
            parameters = new();

            if (specification.Filter is not null) {
                FilterProvider<TEntity> filterProvider = new();
                IOperatorStatement operatorStatement = specification.Filter(filterProvider);
                if (operatorStatement is not null) {
                    clauseBuilder.AppendIfNotEmpty(operatorStatement.StatementString);

                    foreach (ParameterInfo pi in operatorStatement.Parameters) {
                        parameters.Add(pi.ParameterName, pi.Value);
                    }
                }
            }

            OrderProvider<TEntity> orderProvider = new();
            specification.Sorter!(orderProvider);
            string orderByClause = orderProvider.ToString();

            parameters.Add(nameof(specification.PageNumber), specification.PageNumber);
            parameters.Add(nameof(specification.PageSize), specification.PageSize);

            return QueryPagedSqlPattern.Replace("{SelectColumns}", selectColumns)
                .Replace("{WhereClause}", clauseBuilder.StartWhereSeparatorAnd)
                .Replace("{OrderByClause}", orderByClause);
        }

        protected string GetTableAlias<TColumn>(Expression<Func<TEntity, TColumn>> aliasExpr) {
            return ExpressionUtils.GetMember(aliasExpr).First();
        }
    }
}
