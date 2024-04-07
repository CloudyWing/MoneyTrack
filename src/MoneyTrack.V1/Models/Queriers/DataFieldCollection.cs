using System;
using System.Collections.ObjectModel;
using System.Text;

namespace CloudyWing.MoneyTrack.Models.Queriers {
    /// <summary>
    /// Represents a collection of <see cref="DataField"/> objects with a unique key for each field.
    /// </summary>
    public class DataFieldCollection : KeyedCollection<string, DataField> {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFieldCollection"/> class with a case-insensitive key comparer.
        /// </summary>
        public DataFieldCollection() : base(StringComparer.OrdinalIgnoreCase) { }

        /// <summary>
        /// Gets or sets the SQL column filter to apply to the collection.
        /// </summary>
        public ISqlColumnFilter SqlColumnFilter { set; get; }

        /// <summary>
        /// Gets the key for the specified <see cref="DataField"/>.
        /// </summary>
        /// <param name="item">The <see cref="DataField"/> object to get the key for.</param>
        /// <returns>The key of the specified <see cref="DataField"/>.</returns>
        protected override string GetKeyForItem(DataField item) {
            return item.DataKey;
        }

        /// <summary>
        /// Inserts a <see cref="DataField"/> object into the collection at the specified index.
        /// </summary>
        /// <param name="index">The index to insert the <see cref="DataField"/> object at.</param>
        /// <param name="item">The <see cref="DataField"/> object to insert.</param>
        protected override void InsertItem(int index, DataField item) {
            if (!Contains(item.DataKey)) {
                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// Adds a new <see cref="DataField"/> object to the collection with the specified key and SQL column.
        /// </summary>
        /// <param name="key">The key of the data field.</param>
        /// <param name="column">The corresponding SQL column.</param>
        public void Add(string key, string column) {
            Add(new DataField(key, column));
        }

        /// <summary>
        /// Gets the filtered fields in the collection based on the current <see cref="SqlColumnFilter"/>.
        /// </summary>
        public DataFieldCollection FilteredFields {
            get {
                if (SqlColumnFilter != null) {
                    DataFieldCollection newFileds = SqlColumnFilter.Filter(this);

                    return newFileds;
                }
                return this;
            }
        }

        /// <summary>
        /// Removes all items from the collection and sets the <see cref="SqlColumnFilter"/> to null.
        /// </summary>
        protected override void ClearItems() {
            SqlColumnFilter = null;
            base.ClearItems();
        }

        /// <summary>
        /// Returns a string that represents the collection.
        /// </summary>
        /// <returns>A string that represents the collection.</returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            string comma = "";
            foreach (DataField field in FilteredFields) {
                sb.AppendFormat("{0} {1} AS [{2}]", comma, field.SqlColumn, field.DataKey);
                comma = ",";
            }
            return sb.ToString();
        }
    }
}
