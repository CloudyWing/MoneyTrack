using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace System.Web.Mvc {
    /// <summary>
    /// Provides extension methods for generating Ajax-enabled dropdown lists in ASP.NET MVC.
    /// </summary>
    public static class DropDownListExtensions {
        /// <summary>
        /// Generates an Ajax-enabled dropdown list.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="name">The name of the dropdown list.</param>
        /// <param name="group">The grouping identifier.</param>
        /// <param name="ajaxUrl">The URL for AJAX data retrieval.</param>
        /// <param name="optionLabel">The label for the default option.</param>
        /// <param name="nonUsedLabel">The label for non-used items.</param>
        /// <param name="changeCallback">The JavaScript callback function for change events.</param>
        /// <param name="extraParams">Additional parameters to include in the AJAX request.</param>
        /// <param name="htmlAttributes">Additional HTML attributes for the dropdown list.</param>
        /// <returns>The generated MvcHtmlString for the Ajax-enabled dropdown list.</returns>
        public static MvcHtmlString AjaxDropDownList(
            this HtmlHelper htmlHelper,
            string name,
            string group,
            string ajaxUrl,
            string optionLabel = null,
            string nonUsedLabel = null,
            string changeCallback = null,
            object extraParams = null,
            object htmlAttributes = null
        ) {
            return AjaxDropDownList(
                htmlHelper,
                name, group, ajaxUrl, optionLabel, nonUsedLabel, changeCallback,
                extraParams, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
            );
        }

        /// <summary>
        /// Generates an Ajax-enabled dropdown list.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="name">The name of the dropdown list.</param>
        /// <param name="group">The grouping identifier.</param>
        /// <param name="ajaxUrl">The URL for AJAX data retrieval.</param>
        /// <param name="optionLabel">The label for the default option.</param>
        /// <param name="nonUsedLabel">The label for non-used items.</param>
        /// <param name="changeCallback">The JavaScript callback function for change events.</param>
        /// <param name="extraParams">Additional parameters to include in the AJAX request.</param>
        /// <param name="htmlAttributes">Additional HTML attributes for the dropdown list.</param>
        /// <returns>The generated MvcHtmlString for the Ajax-enabled dropdown list.</returns>
        public static MvcHtmlString AjaxDropDownList(
            this HtmlHelper htmlHelper,
            string name,
            string group,
            string ajaxUrl,
            string optionLabel,
            string nonUsedLabel,
            string changeCallback,
            object extraParams,
            IDictionary<string, object> htmlAttributes
        ) {
            return AjaxDropDownListInternal(
                htmlHelper, null,
                name, group, ajaxUrl, optionLabel, nonUsedLabel, changeCallback,
                HtmlHelper.AnonymousObjectToHtmlAttributes(extraParams), htmlAttributes
            );
        }

        /// <summary>
        /// Generates an Ajax-enabled dropdown list using model expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">An expression to identify the model property.</param>
        /// <param name="group">The grouping identifier.</param>
        /// <param name="ajaxUrl">The URL for AJAX data retrieval.</param>
        /// <param name="optionLabel">The label for the default option.</param>
        /// <param name="nonUsedLabel">The label for non-used items.</param>
        /// <param name="changeCallback">The JavaScript callback function for change events.</param>
        /// <param name="extraParams">Additional parameters to include in the AJAX request.</param>
        /// <param name="htmlAttributes">Additional HTML attributes for the dropdown list.</param>
        /// <returns>The generated MvcHtmlString for the Ajax-enabled dropdown list.</returns>
        public static MvcHtmlString AjaxDropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string group,
            string ajaxUrl,
            string optionLabel = null,
            string nonUsedLabel = null,
            string changeCallback = null,
            object extraParams = null,
            object htmlAttributes = null
        ) {
            return AjaxDropDownListFor(
                htmlHelper,
                expression, group, ajaxUrl, optionLabel, nonUsedLabel, changeCallback,
                extraParams, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
            );
        }

        /// <summary>
        /// Generates an Ajax-enabled dropdown list using model expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">An expression to identify the model property.</param>
        /// <param name="group">The grouping identifier.</param>
        /// <param name="ajaxUrl">The URL for AJAX data retrieval.</param>
        /// <param name="optionLabel">The label for the default option.</param>
        /// <param name="nonUsedLabel">The label for non-used items.</param>
        /// <param name="changeCallback">The JavaScript callback function for change events.</param>
        /// <param name="extraParams">Additional parameters to include in the AJAX request.</param>
        /// <param name="htmlAttributes">Additional HTML attributes for the dropdown list.</param>
        /// <returns>The generated MvcHtmlString for the Ajax-enabled dropdown list.</returns>
        public static MvcHtmlString AjaxDropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string group,
            string ajaxUrl,
            string optionLabel,
            string nonUsedLabel,
            string changeCallback,
            object extraParams,
            IDictionary<string, object> htmlAttributes
        ) {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string name = ExpressionHelper.GetExpressionText(expression);

            return AjaxDropDownListInternal(
                htmlHelper, metadata,
                name, group, ajaxUrl, optionLabel, nonUsedLabel, changeCallback,
                HtmlHelper.AnonymousObjectToHtmlAttributes(extraParams), htmlAttributes
            );
        }

        private static MvcHtmlString AjaxDropDownListInternal(
            HtmlHelper htmlHelper,
            ModelMetadata metadata,
            string name,
            string group,
            string ajaxUrl,
            string optionLabel,
            string nonUsedLabel,
            string changeCallback,
            IDictionary<string, object> extraParams,
            IDictionary<string, object> htmlAttributes
        ) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => ajaxUrl);
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => name);

            object defaultValue = metadata?.Model;
            htmlAttributes = AppendFormSelectClass(htmlAttributes);

            TagBuilder select = new TagBuilder("select");
            select.MergeAttributes(htmlAttributes);
            select.GenerateId(name);
            select.MergeAttribute("name", name, true);
            select.MergeAttribute("data-ajax-url", ajaxUrl, true);
            select.MergeAttribute("data-default-value", defaultValue?.ToString(), true);

            if (group != null) {
                select.MergeAttribute("data-select-group", group, true);
            }

            if (optionLabel != null) {
                select.MergeAttribute("data-option-label", optionLabel, true);
            }

            if (nonUsedLabel != null) {
                select.MergeAttribute("data-non-used-label", nonUsedLabel, true);
            }

            if (!string.IsNullOrWhiteSpace(changeCallback)) {
                select.MergeAttribute("data-change-callback", changeCallback, true);
            }

            if (extraParams != null && extraParams.Count() > 0) {
                select.MergeAttribute("data-extra-params", JsonUtils.Serialize(extraParams), true);
            }

            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string actionName,
            object routeValues,
            string optionLabel = null,
            object htmlAttributes = null
        ) {
            return DropDownListFor(htmlHelper, expression, actionName, null, new RouteValueDictionary(routeValues), optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string actionName,
            RouteValueDictionary routeValues = null,
            string optionLabel = null,
            object htmlAttributes = null
        ) {
            return DropDownListFor(htmlHelper, expression, actionName, null, routeValues, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string actionName,
            string controllerName,
            object routeValues,
            string optionLabel = null,
            object htmlAttributes = null
        ) {
            return DropDownListFor(htmlHelper, expression, actionName, controllerName, new RouteValueDictionary(routeValues), optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string actionName,
            string controllerName,
            RouteValueDictionary routeValues = null,
            string optionLabel = null,
            object htmlAttributes = null
        ) {
            string result = htmlHelper.Action(actionName, controllerName, routeValues).ToHtmlString();
            IEnumerable<SelectListItem> selectList = JsonUtils.Deserialize<IEnumerable<SelectListItem>>(result);

            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, AppendFormSelectClass(htmlAttributes));
        }

        private static RouteValueDictionary AppendFormSelectClass(object htmlAttributes) {
            RouteValueDictionary updatedAttributes = new RouteValueDictionary(htmlAttributes);

            if (updatedAttributes.ContainsKey("class") && updatedAttributes["class"] is string @class && !string.IsNullOrEmpty(@class)) {
                updatedAttributes["class"] += " form-select";
            } else {
                updatedAttributes["class"] = "form-select";
            }

            return updatedAttributes;
        }
    }
}
