using System;
using System.Text;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>The string utilities.</summary>
    public static class StringUtils {
        /// <summary>
        /// Generates a random string using the allowed character set.
        /// </summary>
        /// <param name="allowedChars">The allowed character set.</param>
        /// <param name="length">The length of the random string to be generated.</param>
        /// <returns>The randomly generated string.</returns>
        public static string GetRandomString(string allowedChars, int length) {
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++) {
                sb.Append(allowedChars[rd.Next(0, allowedChars.Length)]);
            }
            return sb.ToString();
        }
    }
}
