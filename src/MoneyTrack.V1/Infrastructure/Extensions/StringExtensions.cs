using System.Globalization;

namespace System {
    /// <summary>
    /// Provides extension methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions {
        /// <summary>
        /// Converts the first character of each word in the string to uppercase, and the rest to lowercase.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>The converted string.</returns>
        public static string ToTitleCase(this string str) {
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(str);
        }
    }
}
