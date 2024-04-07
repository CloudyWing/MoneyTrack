namespace CloudyWing.MoneyTrack.Models.Queriers {
    /// <summary>
    /// Represents a single data field with a key and corresponding SQL column.
    /// </summary>
    public struct DataField {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataField"/> struct with the specified key and SQL column.
        /// </summary>
        /// <param name="key">The key of the data field.</param>
        /// <param name="column">The corresponding SQL column.</param>
        public DataField(string key, string column) : this() {
            DataKey = key;
            SqlColumn = column;
        }

        /// <summary>
        /// Gets or sets the key of the data field.
        /// </summary>
        public string DataKey { get; set; }

        /// <summary>
        /// Gets or sets the corresponding SQL column.
        /// </summary>
        public string SqlColumn { get; set; }
    }
}
