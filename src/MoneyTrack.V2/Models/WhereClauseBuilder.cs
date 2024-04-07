using System;
using System.Collections.Generic;
using System.Linq;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Models {

    /// <summary>
    /// Gets the SQL where clause builder.
    /// </summary>
    public sealed class WhereClauseBuilder {
        private readonly IList<string> conitions = new List<string>();

        public string And => ToString(" AND ");

        public string Or => ToString(" OR ");

        public string StartWhereSeparatorAnd => ToString(" AND ", "WHERE");

        public string StartWhereSeparatorOr => ToString(" OR ", "WHERE");

        public string StartAndSeparatorAnd => ToString(" AND ", "AND");

        public string StartAndSeparatorOr => ToString(" OR ", "AND");

        public string StartOrSeparatorAnd => ToString(" AND ", "OR");

        public string StartOrSeparatorOr => ToString(" OR ", "OR");

        public int Length => conitions.Count;

        public WhereClauseBuilder AppendFormat(string condition, params string[] args) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => condition);

            return Append(string.Format(condition, args));
        }

        public WhereClauseBuilder AppendIfNotEmpty(string condition) {
            return string.IsNullOrWhiteSpace(condition) ? this : Append(condition);
        }

        public WhereClauseBuilder Append(string condition) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => condition);

            conitions.Add(condition);
            return this;
        }

        public WhereClauseBuilder Clear() {
            conitions.Clear();
            return this;
        }

        /// <summary>
        /// Creates the Clause.
        /// Format "{startWord} ({item1}{separator}{item2}...)"
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="startWord">The start word.</param>
        /// <exception cref="ArgumentNullException">startWord</exception>
        public string ToString(string separator, string startWord = null) {
            ExceptionUtils.ThrowIfNull(() => separator);

            if (!conitions.Any()) {
                return "";
            }

            string mainStr = $"({string.Join(separator, conitions)})";

            return string.IsNullOrWhiteSpace(startWord) ?
                mainStr : $"{startWord} {mainStr}";
        }

        /// <summary>
        /// Creates the column in multi-value clause.
        /// Format "{column} IN ('{item1}', '{item2}'...)"
        /// </summary>
        /// <param name="column">The column.</param>
        public string ToInClauseString(string column) {
            return $"{column} IN ('{string.Join("', '", conitions)}')";
        }

        public static string GetInClauseString(string column, IEnumerable<string> items) {
            WhereClauseBuilder builder = new WhereClauseBuilder();
            foreach (string item in items) {
                builder.Append(item);
            }
            return builder.ToInClauseString(column);
        }
    }
}
