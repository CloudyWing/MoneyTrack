using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// Provides utility methods for working with DataTables.
    /// </summary>
    public static class DataTableUtils {
        private static readonly IEnumerable<Type> numberType = new Type[] {
            typeof(sbyte), typeof(byte), typeof(short), typeof(ushort),
            typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        };

        /// <summary>
        /// Saves a DataTable to a CSV file in the default temporary directory.
        /// </summary>
        /// <param name="table">The DataTable to save.</param>
        /// <param name="fileName">The name of the CSV file to save.</param>
        public static void SaveToCsv(DataTable table, string fileName) {
            SaveToCsv(table, FileUtils.TemporaryPath, fileName);
        }

        /// <summary>
        /// Saves a DataTable to a CSV file in the specified directory.
        /// </summary>
        /// <param name="table">The DataTable to save.</param>
        /// <param name="path">The directory in which to save the CSV file.</param>
        /// <param name="fileName">The name of the CSV file to save.</param>
        public static void SaveToCsv(DataTable table, string path, string fileName) {
            StringBuilder sb = new StringBuilder();
            List<string> list = new List<string>();

            foreach (DataColumn dc in table.Columns) {
                list.Add(Quote(dc.Caption));
            }
            sb.AppendLine(string.Join(",", list.ToArray()));

            foreach (DataRow dr in table.Rows) {
                list.Clear();

                foreach (DataColumn dc in table.Columns) {
                    if (numberType.Contains(dc.DataType)) {
                        list.Add(dr[dc].ToString());
                    } else {
                        list.Add(Quote(dr[dc].ToString()));
                    }
                }
                sb.AppendLine(string.Join(",", list.ToArray()));
            }
            File.WriteAllText(PathUtils.Combine(path, fileName), sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Encloses a string in double quotes and escapes any existing double quotes.
        /// </summary>
        /// <param name="str">The string to quote.</param>
        /// <returns>The quoted string.</returns>
        private static string Quote(string str) {
            return "\"" + str.Replace(@"""", @"""""") + "\"";
        }
    }
}
