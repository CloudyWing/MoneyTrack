using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.ActionResults;
using CloudyWing.MoneyTrack.Models;

namespace CloudyWing.MoneyTrack.Infrastructure.Filters {
    /// <summary>
    /// An action filter attribute for handling ModelState validation in controller actions.
    /// </summary>
    public sealed class ValidationModelStateAttribute : ActionFilterAttribute {
        /// <summary>
        /// Gets or sets the name of the view to be used in case of validation failure.
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Called before the action method is executed, performs ModelState validation, and handles the result accordingly.
        /// </summary>
        /// <param name="context">The context in which the result is executed.</param>
        public override void OnActionExecuting(ActionExecutingContext context) {
            ViewDataDictionary viewData = context.Controller.ViewData;

            if (!viewData.ModelState.IsValid) {
                HandleValidationFailure(context, viewData);
            }

            base.OnActionExecuting(context);
        }

        private void HandleValidationFailure(ActionExecutingContext context, ViewDataDictionary viewData) {
            if (string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)) {
                context.Result = new HttpNotFoundResult();
            } else {
                if (context.HttpContext.Request.IsAjaxRequest()) {
                    HandleAjaxRequest(context, viewData);
                } else {
                    HandleNonAjaxRequest(context, viewData);
                }
            }
        }

        private void HandleAjaxRequest(ActionExecutingContext context, ViewDataDictionary viewData) {
            context.Result = new JsonNetResult {
                Data = ResponseResult.Fail(viewData.ModelState)
            };
        }

        private void HandleNonAjaxRequest(ActionExecutingContext context, ViewDataDictionary viewData) {
            ViewResult result = new ViewResult {
                ViewName = ViewName ?? context.ActionDescriptor.ActionName,
                TempData = context.Controller.TempData,
                ViewData = new ViewDataDictionary(viewData) {
                    Model = context.ActionParameters.Values.SingleOrDefault()
                },
                ViewEngineCollection = ViewEngines.Engines
            };

            if (viewData.ModelState.Any(x => x.Value.Errors.Count > 0)) {
                CleanUploadedFilesErrors(viewData, result);
            }

            context.Result = result;
        }

        private void CleanUploadedFilesErrors(ViewDataDictionary viewData, ViewResult result) {
            IEnumerable<PropertyInfo> stringProps = result.Model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.PropertyType == typeof(string) && x.CanRead && x.CanWrite);

            foreach (PropertyInfo prop in stringProps) {
                if ((prop.GetValue(result.Model) as string) == "System.Web.HttpPostedFileWrapper") {
                    IEnumerable<string> errorMessages = viewData.ModelState[prop.Name].Errors.Select(x => x.ErrorMessage);
                    viewData.ModelState.Remove(prop.Name);
                    prop.SetValue(result.Model, "");

                    foreach (string errorMessage in errorMessages) {
                        viewData.ModelState.AddModelError(prop.Name, errorMessage);
                    }
                }
            }
        }
    }
}
