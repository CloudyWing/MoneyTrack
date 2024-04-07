using System.IO;
using System.Web;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// Provides utility methods for working with URIs.
    /// </summary>
    public static class UriUtils {

        /// <summary>
        /// Gets the website URL.
        /// </summary>
        /// <returns>The website URL.</returns>
        public static string WebsiteUrl => $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}{HttpContext.Current.Request.ApplicationPath.TrimEnd('/')}/";

        /// <summary>
        /// Combines two paths into a single path.
        /// </summary>
        /// <param name="path1">The first path to combine.</param>
        /// <param name="path2">The second path to combine.</param>
        /// <returns>The combined path.</returns>
        public static string Combine(string path1, string path2) {
            path1 = FixUri(path1);
            path2 = FixUri(path2);
            if (path2.Length == 0) {
                return path1;
            }
            if (path1.Length == 0) {
                return path2;
            }
            char ch = path1[path1.Length - 1];
            if (ch != Path.AltDirectorySeparatorChar && ch != Path.VolumeSeparatorChar) {
                return path1 + Path.AltDirectorySeparatorChar + path2;
            }
            return path1 + path2;
        }

        /// <summary>
        /// Combines multiple paths into a single path.
        /// </summary>
        /// <param name="path">The array of paths to combine.</param>
        /// <returns>The combined path.</returns>
        public static string Combine(params string[] path) {
            string returnPath = string.Empty;

            foreach (string tmp in path) {
                returnPath = Combine(returnPath, tmp);
            }

            return returnPath;
        }

        private static string FixUri(string path) {
            path = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (path.Length > 0 && path[0] == Path.AltDirectorySeparatorChar) {
                path = path.Substring(1);
            }
            return path;
        }
    }
}
