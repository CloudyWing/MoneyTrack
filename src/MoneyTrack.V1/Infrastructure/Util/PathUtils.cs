using System;
using System.IO;
using System.Web;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// A utility class for working with file system paths.
    /// </summary>
    public static class PathUtils {
        /// <summary>
        /// Gets the physical path of the application root directory.
        /// </summary>
        public static string Root => HttpContext.Current.Server.MapPath("~");

        /// <summary>
        /// Ensures that a given path is a valid root path and adds the directory separator character at the end if necessary.
        /// </summary>
        /// <param name="path">The path to fix.</param>
        /// <returns>The fixed path.</returns>
        public static string FixRootPath(string path) {
            if (string.IsNullOrEmpty(path)) {
                return string.Empty;
            }

            if (Path.IsPathRooted(path) && path.Length == 2) {
                path += Path.DirectorySeparatorChar;
            }

            char last = path[path.Length - 1];
            if (string.IsNullOrEmpty(Path.GetExtension(path)) &&
                last != Path.DirectorySeparatorChar &&
                last != Path.AltDirectorySeparatorChar
            ) {
                path += Path.DirectorySeparatorChar;
            }

            return path;
        }

        /// <summary>
        /// Combines multiple paths into a single path.
        /// </summary>
        /// <param name="paths">The paths to combine.</param>
        /// <returns>The combined path.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="paths"/> parameter is null or empty.</exception>
        public static string Combine(params string[] paths) {
            if (paths is null || paths.Length == 0) {
                throw new ArgumentException(nameof(paths));
            }
            string finalPath = FixRootPath(paths[0]);

            for (int i = 1; i < paths.Length; i++) {
                string path = FixRootPath(paths[i]);
                finalPath = Path.Combine(finalPath, path);
            }

            return finalPath;
        }
    }
}
