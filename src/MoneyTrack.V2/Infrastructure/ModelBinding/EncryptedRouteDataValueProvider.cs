using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.ModelBinding {
    /// <summary>
    /// Provides decrypted route data values.
    /// </summary>
    public class EncryptedRouteDataValueProvider : DictionaryValueProvider<object> {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedRouteDataValueProvider"/> class.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        public EncryptedRouteDataValueProvider(ControllerContext controllerContext)
            : base(GetDecryptedRouteValues(controllerContext), CultureInfo.InvariantCulture) {
        }

        private static RouteValueDictionary GetDecryptedRouteValues(ControllerContext controllerContext) {
            RouteValueDictionary routeValues = controllerContext.RouteData.Values;

            if (routeValues.ContainsKey(Constants.UrlParametersKey) &&
                routeValues[Constants.UrlParametersKey] is string encryptedParameters) {
                RouteValueDictionary decryptedRouteValues = DecryptAndParseRouteValues(encryptedParameters);

                if (IsValidExpireTime(decryptedRouteValues)) {
                    AddDecryptedValuesToRouteData(routeValues, decryptedRouteValues);
                }

                routeValues.Remove(Constants.UrlParametersKey);
            }

            return routeValues;
        }

        private static RouteValueDictionary DecryptAndParseRouteValues(string encryptedParameters) {
            string decryptedParameters = CryptographyUtils.Decrypt(HttpUtility.UrlDecode(encryptedParameters), true).Trim('&');

            RouteValueDictionary routeValues = new RouteValueDictionary();

            foreach (string keyValueString in decryptedParameters.Split('&')) {
                string[] keyValueArray = keyValueString.Split('=');
                routeValues.Add(keyValueArray[0], keyValueArray[1]);
            }

            return routeValues;
        }

        private static bool IsValidExpireTime(RouteValueDictionary routeValues) {
            if (routeValues.TryGetValue(Constants.EncryptedExpireTimeKey, out object expireTimeValue)
                && expireTimeValue is string expireTimeString
                && DateTime.TryParseExact(expireTimeString, "yyyyMMddHHmm", null, DateTimeStyles.None, out DateTime expireTime)
                && expireTime >= DateTime.Now
            ) {
                return true;
            }

            return false;
        }

        private static void AddDecryptedValuesToRouteData(RouteValueDictionary routeValues, RouteValueDictionary decryptedRouteValues) {
            foreach (var pair in decryptedRouteValues) {
                if (pair.Key != Constants.EncryptedExpireTimeKey) {
                    routeValues[pair.Key] = pair.Value;
                }
            }
        }
    }
}
