using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace System.Web.Routing {
    public static class UrlHelperExtensions {
        /// <summary>
        /// Generates a query string from route values and appends additional parameters, such as the encrypted expiration time.
        /// </summary>
        /// <param name="helper">The URL helper.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The generated query string with the appended additional parameters.</returns>
        public static string GenerateQueryStringWithExpiration(this UrlHelper helper, RouteValueDictionary routeValues) {
            string url = helper.RouteUrl(routeValues);
            int num = url.IndexOf('?');
            if (num >= 0) {
                string queryStringToAdd = $"{Constants.EncryptedExpireTimeKey}={DateTime.Now.AddHours(1):yyyyMMddHHmm}";
                return url.Substring(num + 1) + $"&{queryStringToAdd}";
            }

            return "";
        }
    }
}
