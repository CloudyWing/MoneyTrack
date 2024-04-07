using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using CloudyWing.MoneyTrack.Infrastructure.ActionResults;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Infrastructure.Controllers {
    public class BasicController : Controller {
        protected string TransferMessage {
            get => TempData["TransferMessage"] as string;
            set => TempData["TransferMessage"] = value;
        }

        /// <summary>
        /// Retrieves strongly-typed data stored in TempData with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <param name="key">The key used to retrieve the data.</param>
        /// <param name="defaultValueGenerator">A function to generate the default value if the key is not found.</param>
        /// <param name="keepForNextRequest">Indicates whether to mark the TempData entry to be kept for the next request.</param>
        /// <returns>
        /// The strongly-typed data stored in TempData, or the default value for the type if the key is not found.
        /// </returns>
        protected T GetInternalTempData<T>(string key, Func<T> defaultValueGenerator = null, bool keepForNextRequest = false) {
            ClearInternalTempData();

            string realKey = $"{GetType().Name}_{key}";
            if (TempData.TryGetValue(realKey, out object data) && data is T typedData) {
                if (keepForNextRequest) {
                    TempData.Keep(realKey);
                }
                return typedData;
            }

            if (defaultValueGenerator is null) {
                return default;
            }

            return defaultValueGenerator();
        }

        /// <summary>
        /// Sets the strongly-typed data in TempData with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the data to store.</typeparam>
        /// <param name="key">The key used to store the data.</param>
        /// <param name="value">The strongly-typed data to be stored in TempData.</param>
        protected void SetInternalTempData<T>(string key, T value) {
            TempData[$"{GetType().Name}_{key}"] = value;
        }

        /// <summary>
        /// Clears unnecessary TempData entries that may have been left by other controllers.
        /// </summary>
        protected void ClearInternalTempData() {
            List<string> keysToRemove = TempData.Keys
                .Where(key => !key.StartsWith(GetType().Name) && key.Contains("Controller_"))
                .ToList();

            foreach (string key in keysToRemove) {
                TempData.Remove(key);
            }
        }

        /// <summary>
        /// 來自黑暗執行序將傳統 Json 改為使用 Json.Net 回傳
        /// </summary>
        protected override JsonResult Json(
            object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior
        ) {
            // 如果是設定限制 Get，且用 Get 取得資料時，呼叫原生的 JsonResult，讓他拋出和原生 Json 一樣的例外
            if (behavior == JsonRequestBehavior.DenyGet
                && string.Equals(Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)
            ) {
                return new JsonResult();
            }

            return new JsonNetResult {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        /// <summary>
        /// Serializes a collection of SelectListItem to JSON and returns it as ContentResult.
        /// </summary>
        /// <param name="selectList">The collection of SelectListItem to serialize.</param>
        /// <returns>ContentResult containing the serialized JSON.</returns>
        protected ContentResult SelectList(IEnumerable<SelectListItem> selectList) {
            return Content(JsonUtils.Serialize(selectList));
        }

        /// <summary>
        /// Redirects to the specified action of a controller with the given route values.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="actionExpr">Expression representing the action to redirect to.</param>
        /// <param name="routeValues">The route values used to generate the URL.</param>
        /// <returns>The result of the action method.</returns>
        protected RedirectToRouteResult RedirectToAction<TController>(Expression<Action<TController>> actionExpr, object routeValues)
            where TController : BasicController {
            return RedirectToAction(actionExpr, routeValues);
        }

        /// <summary>
        /// Redirects to the specified action of a controller with the given route values.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="actionExpr">Expression representing the action to redirect to.</param>
        /// <param name="routeValues">The route values used to generate the URL.</param>
        /// <returns>The result of the action method.</returns>
        protected RedirectToRouteResult RedirectToAction<TController>(Expression<Action<TController>> actionExpr, RouteValueDictionary routeValues)
            where TController : BasicController {
            string actionName = GetActionName(actionExpr);
            string controllerName = Regex.Replace(typeof(TController).Name, "Controller$", "");

            return RedirectToAction(actionName, controllerName, routeValues);
        }

        /// <summary>
        /// Redirects to an action with encrypted query parameters.
        /// </summary>
        /// <param name="actionName">The name of the action to redirect to.</param>
        /// <param name="controllerName">The name of the controller to redirect to.</param>
        /// <param name="routeValues">The route values used to generate the URL.</param>
        /// <returns>The result of the action method.</returns>
        protected RedirectToRouteResult RedirectToActionEncrypted(string actionName, string controllerName, object routeValues) {
            return RedirectToActionEncrypted(actionName, controllerName, new RouteValueDictionary(routeValues));
        }

        /// <summary>
        /// Redirects to an action with encrypted query parameters.
        /// </summary>
        /// <param name="actionName">The name of the action to redirect to.</param>
        /// <param name="controllerName">The name of the controller to redirect to.</param>
        /// <param name="routeValues">The route values used to generate the URL.</param>
        /// <returns>The result of the action method.</returns>
        protected RedirectToRouteResult RedirectToActionEncrypted(string actionName, string controllerName, RouteValueDictionary routeValues) {
            RouteValueDictionary _routeValues = new RouteValueDictionary {
                [Constants.UrlParametersKey] = CryptographyUtils.Encrypt(Url.GenerateQueryStringWithExpiration(routeValues), true)
            };

            return RedirectToAction(actionName, controllerName, _routeValues);
        }

        /// <summary>
        /// Redirects to the specified action of a controller with encrypted query parameters.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="actionExpr">Expression representing the action to redirect to.</param>
        /// <param name="routeValues">The route values used to generate the URL.</param>
        /// <returns>The result of the action method.</returns>
        protected RedirectToRouteResult RedirectToActionEncrypted<TController>(Expression<Action<TController>> actionExpr, object routeValues)
            where TController : BasicController {
            return RedirectToActionEncrypted(actionExpr, routeValues);
        }

        /// <summary>
        /// Redirects to the specified action of a controller with encrypted query parameters.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="actionExpr">Expression representing the action to redirect to.</param>
        /// <param name="routeValues">The route values used to generate the URL.</param>
        /// <returns>The result of the action method.</returns>
        protected RedirectToRouteResult RedirectToActionEncrypted<TController>(Expression<Action<TController>> actionExpr, RouteValueDictionary routeValues)
            where TController : BasicController {
            string actionName = GetActionName(actionExpr);
            string controllerName = Regex.Replace(typeof(TController).Name, "Controller$", "");

            return RedirectToActionEncrypted(actionName, controllerName, routeValues);
        }

        private static string GetActionName<TController>(Expression<Action<TController>> actionExpr) {
            if (actionExpr.Body is MethodCallExpression methodCallExpression) {
                return methodCallExpression.Method.Name;
            }

            throw new ArgumentException("Invalid expression", nameof(actionExpr));
        }
    }
}
