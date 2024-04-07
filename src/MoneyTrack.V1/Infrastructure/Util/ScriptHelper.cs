using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// Provides helper methods for registering and displaying client-side scripts.
    /// </summary>
    public class ScriptHelper {
        private readonly HttpContext httpContext;

        /// <summary>
        /// Initializes a new instance of the ScriptHelper class with the specified HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when the httpContext parameter is null.</exception>
        public ScriptHelper(HttpContext httpContext) {
            this.httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        /// <summary>
        /// Registers a client-side script with the specified key.
        /// </summary>
        /// <param name="script">The script to register.</param>
        /// <param name="key">The key for the script block.</param>
        public void Register(string script, string key = null) {
            if (string.IsNullOrWhiteSpace(key)) {
                key = GetRandomKey();
            }

            IHttpHandler handler = httpContext.CurrentHandler;
            if (handler != null && handler is Page page) {
                ScriptManager sm = ScriptManager.GetCurrent(page);

                if (sm is null) {
                    ClientScriptManager cs = page.ClientScript;
                    cs.RegisterStartupScript(page.GetType(), key, script, true);

                } else {
                    ScriptManager.RegisterStartupScript(page, page.GetType(), key, script, true);
                }
            }
        }

        /// <summary>
        /// Displays an alert dialog box with the specified message and key.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="key">The key for the script block.</param>
        public void Alert(string message, string key = null) {
            message = FixString(message);

            Register($"alert('{message}');", key);
        }

        /// <summary>
        /// Redirects the client to the specified URL after displaying an alert dialog box with the specified message.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <param name="message">The message to display.</param>
        public void RedirectAndAlert(string url, string message) {
            url = FixString(url);
            message = FixString(message);

            HttpContext.Current.Response.Write(
                string.Format(
                    "<script>alert('{1}');location.href='{0}';</script>",
                    url, message)
            );
            HttpContext.Current.Response.End();
        }

        private static string GetRandomKey() {
            return StringUtils.GetRandomString("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 5);
        }

        private string FixString(string str) {
            string[] olds = { "\\", "'", "\"", "\b", "\f", "\n", "\r", "\t" };
            string[] news = { "\\\\", "\\'", "\\\"", "\\b", "\\f", "\\n", "\\r", "\\t" };
            int length = olds.Count();
            for (int i = 0; i < length; i++) {
                str = str.Replace(olds[i], news[i]);
            }
            return str;
        }
    }
}
