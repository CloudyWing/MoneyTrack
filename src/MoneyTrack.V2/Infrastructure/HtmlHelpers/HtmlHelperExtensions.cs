using System.Collections.Generic;
using System.Web.Routing;

namespace System.Web.Mvc {
    /// <remarks>
    /// ref: https://github.com/aspnet/AspNetWebStack/blob/main/src/System.Web.Mvc/Html/DefaultEditorTemplates.cs
    /// </remarks>
    public static class HtmlHelperExtensions {
        private const string HtmlAttributeKey = "htmlAttributes";

        public static IDictionary<string, object> CreateInputHtmlAttributes(this HtmlHelper html, string className, string inputType = null) {
            object htmlAttributesObject = html.ViewContext.ViewData[HtmlAttributeKey];
            if (htmlAttributesObject != null) {
                return MergeHtmlAttributes(htmlAttributesObject, className, inputType);
            }

            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                ["class"] = className
            };

            if (inputType != null) {
                htmlAttributes.Add("type", inputType);
            }

            return htmlAttributes;
        }

        private static IDictionary<string, object> MergeHtmlAttributes(object htmlAttributesObject, string className, string inputType) {
            RouteValueDictionary htmlAttributes = (htmlAttributesObject is IDictionary<string, object> htmlAttributesDict)
                ? new RouteValueDictionary(htmlAttributesDict)
                : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);

            if (htmlAttributes.TryGetValue("class", out object htmlClassName)) {
                htmlAttributes["class"] = htmlClassName?.ToString() + " " + className;
            } else {
                htmlAttributes.Add("class", className);
            }

            if (inputType != null && !htmlAttributes.ContainsKey("type")) {
                htmlAttributes.Add("type", inputType);
            }

            return htmlAttributes;
        }
    }
}
