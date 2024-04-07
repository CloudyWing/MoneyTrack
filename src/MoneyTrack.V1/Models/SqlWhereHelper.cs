using System.Collections.Generic;

namespace CloudyWing.MoneyTrack.Models {
    /// <summary>
    /// Provides helper methods for building SQL WHERE clauses.
    /// </summary>
    public class SqlWhereHelper {
        private readonly List<string> wheres = new List<string>();

        /// <summary>
        /// Gets a string representing the AND combination of all WHERE clauses added to this helper.
        /// </summary>
        public string AndString => string.Join(" AND ", wheres);

        /// <summary>
        /// Gets a string representing the OR combination of all WHERE clauses added to this helper.
        /// </summary>
        public string OrString => string.Format(
                    "{0}{1}{2}",
                    wheres.Count > 1 ? "(" : string.Empty,
                    string.Join(" OR ", wheres),
                    wheres.Count > 1 ? ")" : string.Empty
                );

        /// <summary>
        /// Gets a string representing the AND combination of all WHERE clauses added to this helper, prefixed by "WHERE".
        /// </summary>
        public string WhereAndString {
            get {
                string str = string.Empty;
                if (wheres.Count > 0) {
                    str = @"
                WHERE " + AndString;
                }
                return str;
            }
        }

        /// <summary>
        /// Gets a string representing the OR combination of all WHERE clauses added to this helper, prefixed by "WHERE".
        /// </summary>
        public string WhereOrString {
            get {
                string str = string.Empty;
                if (wheres.Count > 0) {
                    str = @"
                WHERE " + OrString;
                }
                return str;
            }
        }

        /// <summary>
        /// Adds a WHERE clause to this helper.
        /// </summary>
        /// <param name="str">The WHERE clause to add.</param>
        public void Add(string str) {
            wheres.Add(str);
        }
    }
}
