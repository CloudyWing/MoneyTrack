using System.Data;
using System.Text.RegularExpressions;

namespace CloudyWing.MoneyTrack.Models {
    /// <summary>
    /// Represents a field in a database table.
    /// </summary>
    public struct TableField {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableField"/> struct.
        /// </summary>
        /// <param name="model">The data model that the field belongs to.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The <see cref="SqlDbType"/> of the field.</param>
        /// <param name="isPK">Indicates whether the field is a primary key.</param>
        /// <param name="allowNull">Indicates whether the field allows null values.</param>
        /// <param name="hasDefault">Indicates whether the field has a default value.</param>
        /// <param name="isIdentity">Indicates whether the field is an identity column.</param>
        public TableField(
            DataModel model, string name, SqlDbType type, bool isPK, bool allowNull, bool hasDefault, bool isIdentity
        ) : this() {
            Model = model;
            Name = name;
            DbType = type;
            IsPrimaryKey = isPK;
            AllowDBNull = allowNull;
            HasDefault = hasDefault;
            IsIdentity = isIdentity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableField"/> struct.
        /// </summary>
        /// <param name="model">The data model that the field belongs to.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The <see cref="SqlDbType"/> of the field.</param>
        /// <param name="size">The size of the field.</param>
        /// <param name="isPK">Indicates whether the field is a primary key.</param>
        /// <param name="allowNull">Indicates whether the field allows null values.</param>
        /// <param name="hasDefault">Indicates whether the field has a default value.</param>
        public TableField(
            DataModel model, string name, SqlDbType type, int size, bool isPK, bool allowNull, bool hasDefault
        ) : this() {
            Model = model;
            Name = name;
            DbType = type;
            IsPrimaryKey = isPK;
            AllowDBNull = allowNull;
            HasDefault = hasDefault;
            Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableField"/> struct.
        /// </summary>
        /// <param name="model">The data model that the field belongs to.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The <see cref="SqlDbType"/> of the field.</param>
        /// <param name="precision">The precision of the field.</param>
        /// <param name="scale">The scale of the field.</param>
        /// <param name="isPK">Indicates whether the field is a primary key.</param>
        /// <param name="allowNull">Indicates whether the field allows null values.</param>
        /// <param name="hasDefault">Indicates whether the field has a default value.</param>
        /// <param name="isIdentity">Indicates whether the field is an identity column.</param>
        public TableField(
            DataModel model, string name, SqlDbType type, byte precision, byte scale, bool isPK, bool allowNull, bool hasDefault, bool isIdentity
        ) : this() {
            Model = model;
            Name = name;
            DbType = type;
            IsPrimaryKey = isPK;
            AllowDBNull = allowNull;
            HasDefault = hasDefault;
            IsIdentity = isIdentity;
            Precision = precision;
            Scale = scale;
        }

        /// <summary>
        /// Gets the data model of the field.
        /// </summary>
        public DataModel Model { get; private set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <remarks>The Name property is automatically trimmed and enclosed in square brackets if necessary.</remarks>
        public string Name {
            get => name;
            private set {
                Regex regex = new Regex(@"(^\[.*\]$)", RegexOptions.IgnoreCase);
                name = regex.IsMatch(value) ? value.Substring(1, value.Length - 2) : value;
            }
        }

        /// <summary>
        /// Gets the name of the field enclosed in square brackets.
        /// </summary>
        public string NameWithBrackets => $"[{name}]";

        /// <summary>
        /// Gets or sets a value indicating whether the field allows null values.
        /// </summary>
        public bool AllowDBNull { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field is an identity column.
        /// </summary>
        public bool IsIdentity { get; private set; }

        /// <summary>
        /// Gets or sets the maximum size, in bytes, of the field.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Gets or sets the precision of the field.
        /// </summary>
        public byte Precision { get; private set; }

        /// <summary>
        /// Gets or sets the scale of the field.
        /// </summary>
        public byte Scale { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field is a primary key.
        /// </summary>
        public bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// Gets or sets the data type of the field.
        /// </summary>
        public SqlDbType DbType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field has a default value.
        /// </summary>
        public bool HasDefault { get; private set; }

        /// <summary>
        /// Determines whether two TableField objects are equal.
        /// </summary>
        /// <param name="left">The first TableField to compare.</param>
        /// <param name="right">The second TableField to compare.</param>
        /// <returns>true if a and b have the same value; otherwise, false.</returns>
        public static bool operator ==(TableField left, TableField right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two TableField objects are not equal.
        /// </summary>
        /// <param name="left">The first TableField to compare.</param>
        /// <param name="right">The second TableField to compare.</param>
        /// <returns>true if a and b do not have the same value; otherwise, false.</returns>
        public static bool operator !=(TableField left, TableField right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="o">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object o) {
            return base.Equals(o);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// The name.
        /// </returns>
        public override string ToString() {
            return Name;
        }
    }
}
