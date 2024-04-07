namespace CloudyWing.MoneyTrack.Models.Transactions {
    /// <summary>
    /// Represents an action that can be performed and verified.
    /// </summary>
    public interface IActionable {
        /// <summary>
        /// Gets the error message when the action fails.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Verifies if the action can be performed.
        /// </summary>
        /// <returns>
        /// Returns true if the action can be performed, otherwise false.
        /// </returns>
        bool Verify();

        /// <summary>
        /// Executes the action
        /// </summary>
        /// <returns>
        /// Returns true if the action was successfully executed, otherwise false.
        /// </returns>
        bool Execute();
    }
}
