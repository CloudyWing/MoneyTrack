using CloudyWing.MoneyTrack.Infrastructure.Application;

namespace System.Web {
    /// <summary>
    /// Provides extension methods for the <see cref="HttpResponse"/> class.
    /// </summary>
    public static class HttpResponseExtensions {
        /// <summary>
        /// Redirects to self.
        /// </summary>
        /// <param name="response">The response.</param>
        public static void RedirectToSelf(this HttpResponse response) {
            response.Redirect(HttpContext.Current.Request.RawUrl);
        }

        /// <summary>
        /// Redirects to the specified URL, and stores the current URL as the previous page.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponse"/> object.</param>
        /// <param name="url">The URL to redirect to.</param>
        public static void RedirectToNext(this HttpResponse response, string url) {
            UserContext.GetInstance(HttpContext.Current).PageHistory.PreviousPage = url;
            response.Redirect(url);
        }

        /// <summary>
        /// Redirects to the previous page, if available, and removes it from the list of previous pages.
        /// Otherwise, redirects to the home page.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponse"/> object.</param>
        public static void RedirectToPrevious(this HttpResponse response) {
            UserContext user = UserContext.GetInstance(HttpContext.Current);
            string page = user.PageHistory.PreviousPage;

            if (page != null) {
                response.Redirect(page);
            } else {
                response.Redirect("~");
            }
        }
    }
}
