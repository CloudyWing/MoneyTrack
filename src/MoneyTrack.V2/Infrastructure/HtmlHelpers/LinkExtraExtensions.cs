using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace System.Web.Mvc.Html {
    public static class LinkExtraExtensions {
        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString EncryptedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues) {
            return htmlHelper.EncryptedActionLink(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary());
        }

        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString EncryptedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes) {
            return htmlHelper.EncryptedActionLink(linkText, actionName, null, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString EncryptedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues) {
            return htmlHelper.EncryptedActionLink(linkText, actionName, null, routeValues, new RouteValueDictionary());
        }

        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString EncryptedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes) {
            return htmlHelper.EncryptedActionLink(linkText, actionName, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString EncryptedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes) {
            return htmlHelper.EncryptedActionLink(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString EncryptedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes) {
            return htmlHelper.EncryptedActionLink(linkText, actionName, controllerName, null, null, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="fragment">The fragment identifier.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString EncryptedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes) {
            return htmlHelper.EncryptedActionLink(linkText, actionName, controllerName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="fragment">The fragment identifier.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString EncryptedActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => linkText);

            UrlHelper url = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
            RouteValueDictionary encryptedRouteValues = new RouteValueDictionary {
                [Constants.UrlParametersKey] = CryptographyUtils.Encrypt(url.GenerateQueryStringWithExpiration(routeValues), true)
            };

            return MvcHtmlString.Create(HtmlHelper.GenerateLink(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, linkText, null, actionName, controllerName, protocol, hostName, fragment, encryptedRouteValues, htmlAttributes));
        }

        /// <summary>
        /// Generates an encrypted action link wrapped in a button.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues = null) {
            return htmlHelper.ButtonLink(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary());
        }

        /// <summary>
        /// Generates an encrypted action link wrapped in a button.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">An object that contains the parameters for a route.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes) {
            return htmlHelper.ButtonLink(linkText, actionName, null, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Generates an encrypted action link wrapped in a button.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues) {
            return htmlHelper.ButtonLink(linkText, actionName, null, routeValues, new RouteValueDictionary());
        }

        /// <summary>
        /// Generates an encrypted action link wrapped in a button.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes) {
            return htmlHelper.ButtonLink(linkText, actionName, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Generates an encrypted action link wrapped in a button.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues = null, object htmlAttributes = null) {
            RouteValueDictionary _routeValues = routeValues is null
                ? null : new RouteValueDictionary(routeValues);

            RouteValueDictionary _htmlAttributes = htmlAttributes is null
                ? null : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            return htmlHelper.ButtonLink(linkText, actionName, controllerName, _routeValues, _htmlAttributes);
        }

        /// <summary>
        /// Generates an encrypted action link.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes) {
            return htmlHelper.ButtonLink(linkText, actionName, controllerName, null, null, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// Generates an encrypted action link wrapped in a button.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="fragment">The fragment identifier.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes) {
            return htmlHelper.ButtonLink(linkText, actionName, controllerName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Generates an encrypted action link wrapped in a button.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="linkText">The inner text of the link.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName">The host name for the URL.</param>
        /// <param name="fragment">The fragment identifier.</param>
        /// <param name="routeValues">A collection of route values.</param>
        /// <param name="htmlAttributes">An object that contains the HTML attributes.</param>
        /// <returns>The encrypted action link as an MvcHtmlString.</returns>
        public static MvcHtmlString ButtonLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => linkText);

            var attributes = htmlAttributes;

            if (!attributes.ContainsKey("class")) {
                attributes.Add("class", "btn btn-info");
            } else {
                var classValue = attributes["class"].ToString();
                if (!classValue.Split(' ').Contains("btn")) {
                    attributes["class"] += " btn btn-info";
                }
            }

            return htmlHelper.EncryptedActionLink(linkText, actionName, controllerName, protocol, hostName, fragment, routeValues, attributes);
        }
    }
}
