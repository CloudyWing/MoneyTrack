namespace CloudyWing.MoneyTrack.Models.Transactions {
    /// <summary>
    /// Provides a base implementation for the IActionable interface.
    /// </summary>
    public abstract class ActionBase : IActionable {
        protected RecordModel RecordModel => RecordModel.Instance;

        protected CategoryModel CategoryModel => CategoryModel.Instance;

        /// <inheritdoc/>
        public string ErrorMessage { get; protected set; }

        /// <inheritdoc/>
        public virtual bool Verify() {
            return true;
        }

        /// <inheritdoc/>
        public abstract bool Execute();
    }
}
