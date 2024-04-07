using System.Collections.Generic;
using System.Reflection;

namespace CloudyWing.MoneyTrack.Models {
    /// <summary>
    /// The base class for all data models.
    /// </summary>
    public abstract class DataModel {
        private IEnumerable<TableField> cachedFields;

        /// <summary>
        /// Gets the name of the table associated with this data model.
        /// </summary>
        public abstract string TableName { get; }

        /// <summary>
        /// Gets a collection of <see cref="TableField"/> objects that represent the fields in the associated table.
        /// </summary>
        public IEnumerable<TableField> TableFields {
            get {
                if (cachedFields is null) {
                    List<TableField> tf = new List<TableField>();
                    PropertyInfo[] props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (PropertyInfo info in props) {
                        if (info.PropertyType == typeof(TableField)) {
                            tf.Add((TableField)info.GetValue(this, null));
                        }
                    }
                    cachedFields = tf.ToArray();
                }

                return cachedFields;
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// The table name.
        /// </returns>
        public override string ToString() {
            return TableName;
        }
    }
}
