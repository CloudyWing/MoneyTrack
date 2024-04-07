namespace System.Web.Optimization {
    public static class ScriptUtils {
        /// <summary>
        /// Render scripts as deferred
        /// </summary>
        /// <param name="paths">
        /// The paths.
        /// </param>
        /// <returns>
        /// The <see cref="IHtmlString"/>.
        /// </returns>
        public static IHtmlString RenderDefer(params string[] paths) {
            return Scripts.RenderFormat(@"<script src=""{0}"" defer></script>", paths);
        }
    }
}
