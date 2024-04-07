using System;
using System.Diagnostics;
using System.Transactions;

namespace CloudyWing.MoneyTrack.Models.Transactions {
    /// <summary>
    /// Provides a base implementation for performing data transactions.
    /// </summary>
    /// <remark>
    /// TODO 原本是希望將交易行為封裝，並可進行Roll Back，但將個別交易行為獨立出成Actionable的class後，變成有點雞肋的存在，但目前想不到更好的處理架構，就先這樣
    /// </remark>
    public abstract class DataTransaction {
        private TimeSpan? scopeTimeout;

        /// <summary>
        /// Gets the total execution time of the transaction.
        /// </summary>
        public TimeSpan ExecutionTime { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum amount of time for the transaction scope to complete.
        /// </summary>
        public TimeSpan ScopeTimeout {
            get => scopeTimeout ?? (scopeTimeout = new TimeSpan(0, 1, 0)).Value;
            set => scopeTimeout = value;
        }

        /// <summary>
        /// Gets or sets the action to be performed.
        /// </summary>
        public IActionable Action { get; protected set; }

        /// <summary>
        /// Gets the error message when the transaction fails.
        /// </summary>
        public string ErrorMessage { get; private set; } = "";

        /// <summary>
        /// Executes the transaction with the specified action.
        /// </summary>
        /// <returns>
        /// Returns true if the transaction was successfully executed, otherwise false.
        /// </returns>
        public bool Execute() {
            if (Action is null) {
                throw new ArgumentNullException(nameof(Action));
            }
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, ScopeTimeout)) {
                Stopwatch sw = new Stopwatch();
                sw.Reset();
                sw.Start();

                bool isOk = false;
                ErrorMessage = string.Empty;

                if (Action.Verify()) {
                    isOk = Action.Execute();
                } else {
                    ErrorMessage = Action.ErrorMessage;
                }

                if (isOk) {
                    scope.Complete();
                }

                sw.Stop();
                ExecutionTime = sw.Elapsed;

                return isOk;
            }
        }

        /// <summary>
        /// Sets the action.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>The action.</returns>
        protected TAction SetAction<TAction>(TAction action) where TAction : IActionable {
            Action = action;
            return action;
        }
    }
}
