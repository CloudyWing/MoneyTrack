using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace CloudyWing.MoneyTrack.Infrastructure.Logging {
    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    public static class StringExtensions {
        /// <summary>
        /// Pads the left side of the string to a specified total width using the specified encoding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="totalWidth">The total width of the resulting string.</param>
        /// <param name="encoding">The encoding used to calculate the width of the input string.</param>
        /// <returns>A new string that is equivalent to the input string, but left-padded with spaces or a specified character to a total width.</returns>
        public static string PadLeft(this string str, int totalWidth, Encoding encoding) {
            return PadLeft(str, totalWidth, ' ', encoding);
        }

        /// <summary>
        /// Pads the left side of the string to a specified total width using the specified padding character and encoding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="totalWidth">The total width of the resulting string.</param>
        /// <param name="paddingChar">The character used for padding.</param>
        /// <param name="encoding">The encoding used to calculate the width of the input string.</param>
        /// <returns>A new string that is equivalent to the input string, but left-padded with the specified character to a total width.</returns>
        public static string PadLeft(this string str, int totalWidth, char paddingChar, Encoding encoding) {
            int stringTotalWidth = encoding.GetByteCount(str);
            int fixedTotalWidth = totalWidth - stringTotalWidth;
            return fixedTotalWidth >= 0
                ? "".PadLeft(totalWidth - stringTotalWidth, paddingChar) + str
                : str;
        }

        /// <summary>
        /// Pads the right side of the string to a specified total width using the specified encoding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="totalWidth">The total width of the resulting string.</param>
        /// <param name="encoding">The encoding used to calculate the width of the input string.</param>
        /// <returns>A new string that is equivalent to the input string, but right-padded with spaces or a specified character to a total width.</returns>
        public static string PadRight(this string str, int totalWidth, Encoding encoding) {
            return PadRight(str, totalWidth, ' ', encoding);
        }

        /// <summary>
        /// Pads the right side of the string to a specified total width using the specified padding character and encoding.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="totalWidth">The total width of the resulting string.</param>
        /// <param name="paddingChar">The character used for padding.</param>
        /// <param name="encoding">The encoding used to calculate the width of the input string.</param>
        /// <returns>A new string that is equivalent to the input string, but right-padded with the specified character to a total width.</returns>
        public static string PadRight(this string str, int totalWidth, char paddingChar, Encoding encoding) {
            int stringTotalWidth = encoding.GetByteCount(str);
            int fixedTotalWidth = totalWidth - stringTotalWidth;
            return fixedTotalWidth >= 0
                ? str + "".PadRight(totalWidth - stringTotalWidth, paddingChar)
                : str;
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length, using the specified encoding.
        /// 從字串中擷取子字串，起始位置和長度使用指定的編碼。
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <param name="encoding">The encoding used to calculate the width of the input string.</param>
        /// <returns>A new string that is equivalent to the substring of length <paramref name="length"/> that begins at <paramref name="startIndex"/> in this instance.</returns>
        public static string Substring(this string str, int startIndex, int length, Encoding encoding) {
            const string unrecognizedChar = "?";
            const string replacingChar = "翼";
            const string firstHalfCharOfWingInBig5 = "l";

            // Encoding如果為 Big5 的話，罕見字會被轉成 ?，所以先把罕見字替換成一般字再做切割
            Dictionary<int, string> rareWordMaps = new Dictionary<int, string>();
            for (int i = 0; i < str.Length; i++) {
                string charStr = str.Substring(i, 1);
                if (charStr != unrecognizedChar && encoding.GetString(encoding.GetBytes(charStr)) == unrecognizedChar) {
                    rareWordMaps.Add(i, charStr);
                    str = str.Substring(0, i) + replacingChar + str.Substring(i + 1);
                }
            }

            string subStr = encoding.GetString(encoding.GetBytes(str), startIndex, length);
            foreach (var pair in rareWordMaps) {
                if (pair.Key <= subStr.Length - 1 && subStr.Substring(pair.Key, 1) == replacingChar) {
                    subStr = subStr.Substring(0, pair.Key) + pair.Value + subStr.Substring(pair.Key + 1);
                } else if (pair.Key <= subStr.Length - 1 && subStr.Substring(pair.Key, 1) == firstHalfCharOfWingInBig5) {
                    subStr = subStr.Substring(0, pair.Key) + "?" + subStr.Substring(pair.Key + 1);
                }
            }

            return subStr;
        }

        /// <summary>
        /// Converts the string to proper case, handling numeric character references.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>A new string that is equivalent to the input string, but with the first character of each word in uppercase and the rest in lowercase.</returns>
        public static string ToProper(this string str) {
            return FromNumCharReference(Strings.StrConv(ToNumCharReference(str), VbStrConv.ProperCase));
        }

        /// <summary>
        /// Converts the string to single-byte characters, handling numeric character references.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>A new string that is equivalent to the input string, but with all characters converted to single-byte width.</returns>
        public static string ToSingleByteChar(this string str) {
            return FromNumCharReference(Strings.StrConv(ToNumCharReference(str), VbStrConv.Narrow));
        }

        /// <summary>
        /// Converts the string to double-byte characters, handling numeric character references.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>A new string that is equivalent to the input string, but with all characters converted to double-byte width.</returns>
        public static string ToDoubleByteChar(this string str) {
            return FromNumCharReference(Strings.StrConv(ToNumCharReference(str), VbStrConv.Wide));
        }

        /// <summary>
        /// Converts the string to numeric character references.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>A new string that is equivalent to the input string, but with each character represented by its Unicode code point.</returns>
        public static string ToNumCharReference(this string str) {
            if (string.IsNullOrWhiteSpace(str)) {
                return str;
            }

            Encoding big5 = Encoding.GetEncoding("big5");

            StringBuilder sb = new StringBuilder();

            foreach (char c in str) {
                int unicode = c;

                if (c.ToString() == big5.GetString(big5.GetBytes(new char[] { c }))) {
                    sb.Append(c);
                } else {
                    sb.Append("&#" + unicode + ";");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts the string from numeric character references.
        /// </summary>
        /// <param name="str">The input string with numeric character references.</param>
        /// <returns>A new string that is equivalent to the input string, but with numeric character references replaced by their corresponding Unicode characters.</returns>
        public static string FromNumCharReference(this string str) {
            if (string.IsNullOrWhiteSpace(str)) {
                return str;
            }

            return Regex.Replace(str, "&#(?<ncr>\\d+?);", (m) => {
                return Convert.ToChar(int.Parse(m.Groups["ncr"].Value)).ToString();
            });
        }

        /// <summary>
        /// Reverses the characters in the string.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>A new string that is equivalent to the input string, but with its characters in reverse order.</returns>
        public static string Reverse(this string str) {
            return new string(str.AsEnumerable().Reverse().ToArray());
        }

        /// <summary>
        /// Searches for the last occurrence of a character in the reversed string.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="value">The character to locate in the reversed string.</param>
        /// <returns>The zero-based index position of the last occurrence of <paramref name="value"/> within the reversed string, or -1 if not found.</returns>
        public static int ReverseIndexOf(this string str, char value) {
            return Reverse(str).IndexOf(value);
        }

        /// <summary>
        /// Searches for the last occurrence of a character in the reversed string, starting from a specified index.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="value">The character to locate in the reversed string.</param>
        /// <param name="startIndex">The index to start searching from.</param>
        /// <returns>The zero-based index position of the last occurrence of <paramref name="value"/> within the reversed string, starting from <paramref name="startIndex"/>, or -1 if not found.</returns>
        public static int ReverseIndexOf(this string str, char value, int startIndex) {
            return Reverse(str).IndexOf(value, startIndex);
        }

        /// <summary>
        /// Extracts the substring before the first occurrence of the specified separator.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="separator">The separator character.</param>
        /// <returns>The substring before the first occurrence of the specified separator.</returns>
        public static string SplitStart(this string str, char separator) {
            return str.Substring(0, str.Length - Reverse(str).IndexOf(separator) - 1).TrimEnd(separator);
        }

        /// <summary>
        /// Extracts the substring after the last occurrence of the specified separator, or an empty string if not found.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="separator">The separator character.</param>
        /// <returns>The substring after the last occurrence of the specified separator, or an empty string if not found.</returns>
        public static string SplitEnd(this string str, char separator) {
            return str.Substring(str.Length - Reverse(str).IndexOf(separator) - 1).TrimStart(separator);
        }
    }

}
