using System;
using CloudyWing.Enumeration.Abstractions;

namespace CloudyWing.MoneyTrack.Models.Enumerations {
    /// <summary>
    /// Represents the different comparison modes that can be used in a database query.
    /// </summary>
    public class CompareMode : EnumerationBase<CompareMode, string> {
        private static readonly Lazy<CompareMode> equal = new Lazy<CompareMode>(() => new CompareMode("=", string.Empty));
        private static readonly Lazy<CompareMode> notEqual = new Lazy<CompareMode>(() => new CompareMode("<>", "Not_"));
        private static readonly Lazy<CompareMode> like = new Lazy<CompareMode>(() => new CompareMode("LIKE", "Like_"));
        private static readonly Lazy<CompareMode> notLike = new Lazy<CompareMode>(() => new CompareMode("NOT LIKE", "NotLike_"));
        private static readonly Lazy<CompareMode> greater = new Lazy<CompareMode>(() => new CompareMode(">", "Greater_"));
        private static readonly Lazy<CompareMode> greaterOrEqual = new Lazy<CompareMode>(() => new CompareMode(">=", "GreaterOrEqual_"));
        private static readonly Lazy<CompareMode> less = new Lazy<CompareMode>(() => new CompareMode("<", "Less_"));
        private static readonly Lazy<CompareMode> lessOrEqual = new Lazy<CompareMode>(() => new CompareMode("<=", "LessOrEqual_"));

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareMode"/> class with the specified value and prefix.
        /// </summary>
        /// <param name="value">The value of the comparison mode.</param>
        /// <param name="prefix">The prefix to use for this comparison mode.</param>
        private CompareMode(string value, string prefix) : base(value, value) {
            Prefix = prefix;
        }

        /// <summary>
        /// Gets the comparison mode for equal (=) comparison.
        /// </summary>
        public static CompareMode Equal => equal.Value;

        /// <summary>
        /// Gets the comparison mode for not equal (&lt;&gt;) comparison.
        /// </summary>
        public static CompareMode NotEqual => notEqual.Value;

        /// <summary>
        /// Gets the comparison mode for LIKE comparison.
        /// </summary>
        public static CompareMode Like => like.Value;

        /// <summary>
        /// Gets the comparison mode for NOT LIKE comparison.
        /// </summary>
        public static CompareMode NotLike => notLike.Value;

        /// <summary>
        /// Gets the comparison mode for greater than (>) comparison.
        /// </summary>
        public static CompareMode Greater => greater.Value;

        /// <summary>
        /// Gets the comparison mode for greater than or equal to (>=) comparison.
        /// </summary>
        public static CompareMode GreaterOrEqual => greaterOrEqual.Value;

        /// <summary>
        /// Gets the comparison mode for less than (&lt;) comparison.
        /// </summary>
        public static CompareMode Less => less.Value;

        /// <summary>
        /// Gets the comparison mode for less than or equal to (&lt;=) comparison.
        /// </summary>
        public static CompareMode LessOrEqual => lessOrEqual.Value;

        /// <summary>
        /// Gets the prefix used for this comparison mode.
        /// </summary>
        public string Prefix { get; private set; }
    }
}
