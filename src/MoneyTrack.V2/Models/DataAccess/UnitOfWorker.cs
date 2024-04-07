using System;
using System.Collections.ObjectModel;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess.Entities;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public class UnitOfWorker : IDisposable {
        private readonly ModificationCommandCollection modificationCommands = new ModificationCommandCollection();
        private DatabaseContext dbContext;
        private bool disposedValue;

        public UnitOfWorker(DatabaseContext dbContext) {
            ExceptionUtils.ThrowIfNull(() => dbContext);

            this.dbContext = dbContext;
            Categories = new GenericRepository<CategoryCondition, Category, long>(this);
            Records = new GenericRepository<RecordCondition, Record, long>(this);
        }

        public DatabaseContext DbContext => dbContext;

        public GenericRepository<CategoryCondition, Category, long> Categories { get; }

        public GenericRepository<RecordCondition, Record, long> Records { get; }

        public void AddModificationCommand(ModificationCommand command) {
            modificationCommands.Add(command);
        }

        public int SaveChanges() {
            int i = 0;
            foreach (ModificationCommand action in modificationCommands) {
                i += action.CommandAction();
            }

            return i;
        }

        #region Dispose
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    dbContext?.Dispose();
                }

                dbContext = null;
                disposedValue = true;
            }
        }

        public void Dispose() {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWorker() {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: false);
        }
        #endregion

        private class ModificationCommandCollection : Collection<ModificationCommand> { }
    }
}
