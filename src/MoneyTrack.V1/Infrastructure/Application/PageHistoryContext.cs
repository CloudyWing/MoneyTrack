using System;
using System.Collections.Generic;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.Application {
    /// <summary>
    /// The <see cref="PageHistoryContext"/> class provides a way to track the URL of the previous page that the user visited.
    /// </summary>
    [Serializable]
    public class PageHistoryContext {
        private readonly UserContext userContext;
        private readonly Dictionary<string, string> previousPages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="PageHistoryContext"/> class.
        /// </summary>
        /// <param name="userContext">The user context.</param>
        /// <exception cref="ArgumentNullException">userContext</exception>
        public PageHistoryContext(UserContext userContext) {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        /// <summary>
        /// Gets the URL of the previous page that the user visited.
        /// </summary>
        public string PreviousPage {
            get {
                string currentPageUrl = userContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Path);
                if (previousPages.ContainsValue(currentPageUrl)) {
                    foreach (KeyValuePair<string, string> pair in previousPages) {
                        if (pair.Value == currentPageUrl) {
                            return pair.Key;
                        }
                    }
                }
                return null;
            }
            set {
                string currentPageUrl = userContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Path);
                previousPages.AddOrReplace(currentPageUrl, ResolveUrl(value));
            }
        }

        /// <summary>
        /// Removes a previous page from the list of visited pages.
        /// </summary>
        /// <param name="url">The URL of the previous page to remove.</param>
        public void RemovePreviousPage(string url) {
            previousPages.Remove(ResolveUrl(url));
        }

        private string ResolveUrl(string url) {
            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri) {
                Uri basicUri = new Uri(userContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority));
                Uri basicAppUri = new Uri(UriUtils.WebsiteUrl);
                if (uri.OriginalString.StartsWith("/")) {
                    uri = new Uri(basicUri, uri.OriginalString);
                } else if (uri.OriginalString.StartsWith("~/")) {
                    uri = new Uri(basicAppUri, uri.OriginalString.Substring(2));
                } else {
                    Uri currentUri = new Uri(userContext.HttpContext.Request.Url, userContext.HttpContext.Request.RawUrl);
                    string currentDirectory = currentUri.AbsolutePath.Substring(0, currentUri.AbsolutePath.LastIndexOf('/'));
                    uri = new Uri(currentDirectory + "/" + uri.OriginalString, UriKind.Relative);

                    return new Uri(new Uri(UriUtils.WebsiteUrl), uri).GetLeftPart(UriPartial.Path).Replace(".aspx", "");
                }
                return uri.ToString().TrimEnd('/').Replace(".aspx", "");
            } else {
                return uri.GetLeftPart(UriPartial.Path).Replace(".aspx", "");
            }
        }
    }
}
