using System.Data.Common;
using System.Reflection;
using Dapper;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public class GenericRepository<TCondition, TEntity, TKey>(UnitOfWorker unitOfWorker)
        : QueryableRepository<TCondition, TEntity, TKey>(unitOfWorker)
        where TCondition : class, new()
        where TEntity : class {
        public void Add(TEntity entity) {
            ModificationCommand command = new(
                ModificationType.Insert, async () => {
                    return await AddActionAsync(entity, UnitOfWorker.DbContext.CurrentTransaction);
                });

            UnitOfWorker.AddModificationCommand(command);
        }

        protected virtual async Task<int> AddActionAsync(TEntity entity, DbTransaction? transaction = null) {
            object? id = await UnitOfWorker.DbContext.DbConnection.InsertAsync<TKey, TEntity>(
                entity, transaction
            );

            List<PropertyInfo> props = entity.GetType().GetProperties()
                .Where(x => x.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() != null)
                .ToList();

            if (props.Count == 1) {
                props.Single().SetValue(entity, id);
            }

            return 1;
        }

        public void Update(TEntity entity) {
            ModificationCommand command = new(
                ModificationType.Update,
                async () => await UpdateActionAsync(entity, UnitOfWorker.DbContext.CurrentTransaction)
            );

            UnitOfWorker.AddModificationCommand(command);
        }

        protected virtual async Task<int> UpdateActionAsync(TEntity entity, DbTransaction? transaction = null) {
            return await UnitOfWorker.DbContext.DbConnection.UpdateAsync(
                entity, transaction
            );
        }

        public void Delete(TKey key) {
            ModificationCommand command = new(
                ModificationType.Delete,
                async () => await DeleteActionAsync(key, UnitOfWorker.DbContext.CurrentTransaction)
            );

            UnitOfWorker.AddModificationCommand(command);
        }

        protected virtual async Task<int> DeleteActionAsync(TKey key, DbTransaction? transaction = null) {
            return await UnitOfWorker.DbContext.DbConnection.DeleteAsync<TEntity>(
                key, transaction
            );
        }
    }
}
