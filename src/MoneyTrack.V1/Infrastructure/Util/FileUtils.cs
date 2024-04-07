using System.IO;
using System.Web;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// Utility class for file-related operations.
    /// </summary>
    public static class FileUtils {
        /// <summary>
        /// The path to the directory for temporary files.
        /// </summary>
        public static string TemporaryPath => PathUtils.Combine(PathUtils.Root, "TemporaryFiles");

        /// <summary>
        /// Get a non-repeating file name in the specified directory.
        /// </summary>
        /// <param name="directoryName">The directory to search for the file name in.</param>
        /// <param name="fileName">The original file name to check for repeats.</param>
        /// <returns>A non-repeating file name in the specified directory.</returns>
        public static string GetFileNameNonRepeat(string directoryName, string fileName) {
            string newFileName = fileName;

            for (int i = 0; File.Exists(PathUtils.Combine(directoryName, newFileName)); i++) {
                newFileName = string.Format(
                    "{0}({1}).{2}",
                    Path.GetFileNameWithoutExtension(newFileName),
                    i,
                    Path.GetExtension(newFileName)
                );
            }
            return newFileName;
        }

        /// <summary>
        /// Creates a new file and ensures that it does not overwrite an existing file.
        /// </summary>
        /// <param name="path">The path to the new file.</param>
        /// <returns>A FileStream object for the new file.</returns>
        public static FileStream CreateNonOverwrite(string path) {
            string directoryName = Path.GetDirectoryName(path);
            string fileName = GetFileNameNonRepeat(directoryName, Path.GetFileName(path));
            string newPath = PathUtils.Combine(directoryName, fileName);
            return File.Create(newPath);
        }

        /// <summary>
        /// Downloads the specified file from the temporary file directory.
        /// </summary>
        /// <param name="fileName">The name of the file to download.</param>
        public static void Download(string fileName) {
            Download(TemporaryPath, fileName);
        }

        /// <summary>
        /// Downloads the specified file from the specified directory.
        /// </summary>
        /// <param name="filePath">The path to the directory containing the file to download.</param>
        /// <param name="fileName">The name of the file to download.</param>
        public static void Download(string filePath, string fileName) {
            FileInfo file = new FileInfo(PathUtils.Combine(filePath, fileName));

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpContext.Current.Server.UrlEncode(file.Name));
            HttpContext.Current.Response.AddHeader("Content-Length", file.Length.ToString());
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.WriteFile(file.FullName);
            HttpContext.Current.Response.Flush();
        }

        /// <summary>
        /// Deletes the specified file from the temporary file directory.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        public static void Delete(string fileName) {
            string file = PathUtils.Combine(TemporaryPath, fileName);
            if (File.Exists(file)) {
                File.Delete(file);
            }
        }
    }
}
