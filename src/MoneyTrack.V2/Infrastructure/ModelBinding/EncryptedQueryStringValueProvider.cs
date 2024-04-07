using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    /// <summary>
    /// Provides access to encrypted query string values.
    /// </summary>
    public class EncryptedQueryStringValueProvider : NameValueCollectionValueProvider {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedQueryStringValueProvider"/> class.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        public EncryptedQueryStringValueProvider(ControllerContext controllerContext)
            : base(DecryptAndConvertToNameValueCollection(controllerContext.HttpContext.Request.QueryString[Constants.UrlParametersKey]), CultureInfo.InvariantCulture) {
        }

        private static NameValueCollection DecryptAndConvertToNameValueCollection(string encryptedParameters) {
            if (string.IsNullOrWhiteSpace(encryptedParameters)) {
                return new NameValueCollection();
            }

            NameValueCollection nameValues = HttpUtility.ParseQueryString(CryptographyUtils.Decrypt(HttpUtility.UrlDecode(encryptedParameters ?? ""), true));
            if (nameValues.AllKeys.Length == 0) {
                return nameValues;
            }

            if (!nameValues.AllKeys.Contains(Constants.EncryptedExpireTimeKey)
                || !DateTime.TryParseExact(nameValues[Constants.EncryptedExpireTimeKey], "yyyyMMddHHmm", null, DateTimeStyles.None, out DateTime expireTime)
                || expireTime < DateTime.Now) {
                return new NameValueCollection();
            }

            nameValues.Remove(Constants.EncryptedExpireTimeKey);

            return nameValues;
        }
    }
}
