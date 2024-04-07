using System;
using System.Data;
using System.Data.Common;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Dapper;

namespace CloudyWing.MoneyTrack.Models.DataAccess {
    public class DatabaseContext : IDisposable {
        private readonly DbProviderFactory dbProviderFactory;
        private readonly string connectionString;
        private bool disposedValue;

        public DatabaseContext(DbProviderFactory dbProviderFactory, string connectionString) {
            ExceptionUtils.ThrowIfNull(() => dbProviderFactory);
            ExceptionUtils.ThrowIfNull(() => connectionString);

            this.dbProviderFactory = dbProviderFactory ?? throw new ArgumentNullException(nameof(dbProviderFactory));
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            DbConnection = CreateDbConnection();
        }

        public DbConnection DbConnection { get; private set; }

        public DbTransaction CurrentTransaction { get; private set; }

        private DbConnection CreateDbConnection() {
            if (DbConnection is null) {
                DbConnection = dbProviderFactory.CreateConnection();
            }

            OpenDbConnection();

            return DbConnection;
        }

        public void OpenDbConnection() {
            if (DbConnection.State == ConnectionState.Broken) {
                DbConnection.Close();
            }

            // 避免 Close Command 時，Connection 被重置
            // 為預防萬一，沒值就重新設定
            if (string.IsNullOrWhiteSpace(DbConnection.ConnectionString)) {
                DbConnection.ConnectionString = connectionString;
            }

            if (DbConnection.State != ConnectionState.Open) {
                CurrentTransaction?.Dispose();
                DbConnection.Open();
            }
        }

        public DbTransaction BeginTransaction() {
            return BeginTransaction(IsolationLevel.Unspecified);
        }

        public DbTransaction BeginTransaction(IsolationLevel isolationLevel) {
            OpenDbConnection();
            CurrentTransaction = DbConnection.BeginTransaction(isolationLevel);
            return CurrentTransaction;
        }

        public void CommitTransaction() {
            if (CurrentTransaction is null) {
                throw new InvalidOperationException();
            }

            CurrentTransaction.Commit();
        }

        public void RollbackTransaction() {
            if (CurrentTransaction is null) {
                throw new InvalidOperationException();
            }

            CurrentTransaction.Rollback();
        }

        #region Dispose
        private void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    DbConnection.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose() {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        static DatabaseContext() {
            SqlMapper.AddTypeHandler(new GuidHandler());
        }
        #endregion

        private class GuidHandler : SqlMapper.TypeHandler<Guid> {
            public override void SetValue(IDbDataParameter parameter, Guid value) {
                parameter.Value = value;
            }

            public override Guid Parse(object value) {
                return Guid.Parse((string)value);
            }
        }
    }
}
