using System.Collections.Specialized;

namespace CloudyWing.MoneyTrack.Models.Queriers {
    /// <summary>
    /// Implementation of ISqlColumnFilter that filters data fields by their keys.
    /// </summary>
    public class SqlColumnFilter : ISqlColumnFilter {
        private readonly StringDictionary fields = new StringDictionary();

        /// <summary>
        /// Determines whether a field with a specific key is allowed by the filter.
        /// </summary>
        /// <param name="key">The key of the field to check.</param>
        /// <returns>True if the field is allowed; false otherwise.</returns>
        public bool AllowField(string key) {
            return fields.ContainsKey(key);
        }

        /// <summary>
        /// Adds a field key to the filter.
        /// </summary>
        /// <param name="key">The key of the field to add.</param>
        public void AddField(string key) {
            fields.Add(key, key);
        }

        /// <summary>
        /// Removes a field key from the filter.
        /// </summary>
        /// <param name="key">The key of the field to remove.</param>
        public void RemoveField(string key) {
            fields.Remove(key);
        }

        /// <inheritdoc/>
        public DataFieldCollection Filter(DataFieldCollection fields) {

            if (this.fields.Count > 0) {
                DataFieldCollection newFileds = new DataFieldCollection();
                foreach (DataField field in fields) {
                    if (AllowField(field.DataKey)) {
                        newFileds.Add(field.DataKey, field.SqlColumn);
                    }
                }
                return newFileds;
            }

            return fields;
        }
    }
}
