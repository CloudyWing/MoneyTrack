using System.ComponentModel.DataAnnotations;
using System.Reflection;
using CloudyWing.MoneyTrack.Infrastructure.DependencyInjection;
using CloudyWing.MoneyTrack.Infrastructure.Filters;
using CloudyWing.MoneyTrack.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CloudyWing.MoneyTrack.Models.Application {
    /// <summary>
    /// Basic class for page models.
    /// </summary>
    public class PageModelBase : PageModel {
        private LazyServiceProvider? lazyServiceProvider;

        /// <summary>
        /// Gets or sets the title to be displayed in the page header.
        /// </summary>
        [ViewData]
        public virtual string? Title => GetTitle(MenuViewModel.Instance.ChildMenus!);

        /// <summary>
        /// Gets or sets the menu key.
        /// </summary>
        /// <value>
        [ViewData]
        public virtual string? MenuKey { get; }

        private string? GetTitle(IList<MenuViewModel> menus) {
            foreach (MenuViewModel menu in menus) {
                if (menu.MenuKey == MenuKey) {
                    return menu.Title;
                }

                if (menu.ChildMenus is not null) {
                    string? title = GetTitle(menu.ChildMenus);
                    if (title is not null) {
                        return title;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the lazy service provider for lazy service resolution.
        /// </summary>
        protected LazyServiceProvider LazyServiceProvider {
            get {
                lazyServiceProvider ??= new LazyServiceProvider(PageContext.HttpContext.RequestServices);

                return lazyServiceProvider;
            }
        }

        /// <summary>
        /// Gets or sets the status notification.
        /// </summary>
        public NotificationViewModel? StatusNotification {
            get {
                string? value = TempData["StatusNotification"]?.ToString();
                return value is null ? null : JsonUtils.Deserialize<NotificationViewModel>(value);

            }
            set => TempData["StatusNotification"] = JsonUtils.Serialize(value);
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
        protected T? GetInternalTempData<T>(string key, Func<T>? defaultValueGenerator = null, bool keepForNextRequest = false) {
            ClearInternalTempData();

            string realKey = GetFullKey(key);
            if (TempData.TryGetValue(realKey, out object? data) && data is not null) {
                if (keepForNextRequest) {
                    TempData.Keep(realKey);
                }
                return  JsonUtils.Deserialize<T>(data.ToString()!);
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
            TempData[GetFullKey(key)] = JsonUtils.Serialize(value);
        }

        /// <summary>
        /// Clears unnecessary TempData entries that may have been left by other controllers.
        /// </summary>
        protected void ClearInternalTempData() {
            List<string> keysToRemove = TempData.Keys
                .Where(key => !key.StartsWith(GetType().FullName!) && key.Contains("Model_"))
                .ToList();

            foreach (string key in keysToRemove) {
                TempData.Remove(key);
            }
        }

        private string GetFullKey(string key) {
            return $"{GetType().FullName}_{key}";
        }

        /// <summary>
        /// Creates a JsonResult with the specified data.
        /// </summary>
        /// <param name="data">The data to be serialized.</param>
        /// <returns>A JsonResult object.</returns>
        protected virtual JsonResult Json(object? data) {
            return new(data);
        }

        /// <summary>
        /// Called before the page handler is executed.
        /// </summary>
        /// <param name="context">The page handler executing context.</param>
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context) {
            string httpMethod = context.HttpContext.Request.Method;

            bool isValid = true;

            MethodInfo handlerMethod = context.HandlerMethod!.MethodInfo;
            ParameterInfo[] parameters = handlerMethod.GetParameters();

            foreach (ParameterInfo parameter in parameters) {
                object? argument = null;

                if (context.HandlerArguments.TryGetValue(parameter.Name!, out object? value)) {
                    argument = value;
                } else if (parameter.HasDefaultValue) {
                    argument = parameter.DefaultValue;
                }

                EvaluateValidationAttributes(parameter, argument, context.ModelState);
            }

            ValidationExecutionAttribute? validationExecutionAttribute = handlerMethod
                .GetCustomAttributes(typeof(ValidationExecutionAttribute), true)
                .SingleOrDefault() as ValidationExecutionAttribute;

            bool stopAutoValidation = handlerMethod.GetCustomAttributes(typeof(StopAutoValidationAttribute), true).Length != 0;
            List<string> validatePropertyNames = handlerMethod.GetCustomAttributes(typeof(ValidationPropertyAttribute), true)
                .Cast<ValidationPropertyAttribute>()
                .SelectMany(x => x.PropertyName.Split(","))
                .ToList();

            InvokeValidationAction(validationExecutionAttribute?.OnExecutingAction, context);

            if (!stopAutoValidation) {
                if (validatePropertyNames.Count != 0) {
                    ModelState.Clear();

                    foreach (string propertyName in validatePropertyNames) {
                        PropertyInfo propertyInfo = GetType().GetProperty(propertyName)!;
                        if (propertyInfo is not null) {
                            if (!TryValidateModel(propertyInfo.GetValue(this)!, propertyName)) {
                                isValid = false;
                            }
                        }
                    }
                } else {
                    isValid = ModelState.IsValid;
                }

                if (!isValid) {
                    InvokeValidationAction(validationExecutionAttribute?.OnFailExecutedAction, context);

                    if (!string.IsNullOrWhiteSpace(validationExecutionAttribute?.OnFailResultAction)) {
                        context.Result = InvokeValidationActionWithResult(validationExecutionAttribute.OnFailResultAction, context);
                        return;
                    }

                    if (httpMethod.Equals(HttpMethods.Get, StringComparison.OrdinalIgnoreCase)) {
                        context.Result = new NotFoundResult();
                    } else if (httpMethod.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase)
                          && IsAjaxRequest(context.HttpContext.Request)) {
                        context.Result = new JsonResult(new ApplicationResult {
                            IsOk = false,
                            Message = ModelState.Values.Where(v => v.Errors.Any()).SelectMany(v => v.Errors).Select(v => v.ErrorMessage).First()
                        });
                    } else {
                        context.Result = new PageResult();
                    }
                }
            }

            base.OnPageHandlerExecuting(context);

            if (isValid) {
                InvokeValidationAction(validationExecutionAttribute?.OnSuccessExecutedAction, context);
            }
        }

        private static void EvaluateValidationAttributes(ParameterInfo parameter, object? argument, ModelStateDictionary modelState) {
            IEnumerable<CustomAttributeData> validationAttributes = parameter.CustomAttributes;

            foreach (CustomAttributeData attributeData in validationAttributes) {
                Attribute? attributeInstance = parameter.GetCustomAttribute(attributeData.AttributeType);

                ValidationAttribute? validationAttribute = attributeInstance as ValidationAttribute;

                if (validationAttribute is not null) {
                    bool isValid = validationAttribute.IsValid(argument);
                    if (!isValid) {
                        modelState.AddModelError(parameter.Name!, validationAttribute.FormatErrorMessage(parameter.Name!));
                    }
                }
            }
        }

        private void InvokeValidationAction(string? actionName, PageHandlerExecutingContext context) {
            if (actionName is null) {
                return;
            }

            MethodInfo? mi = GetMethod(actionName);

            if (mi is not null) {
                object? result = mi.Invoke(this, mi.GetParameters().Length > 0 ? new object[] { context } : null);

                if (result is Task taskResult) {
                    taskResult.GetAwaiter().GetResult();
                }
                return;
            }

            throw new InvalidOperationException($"The method {actionName} is not found.");
        }

        private IActionResult InvokeValidationActionWithResult(string actionName, PageHandlerExecutingContext context) {
            MethodInfo? mi = GetMethod(actionName);

            if (mi is not null) {
                object? result = mi.Invoke(this, mi.GetParameters().Length > 0 ? new object[] { context } : null);

                if (result is IActionResult _result) {
                    return _result;
                } else if (result is Task<IActionResult> taskResult) {
                    return taskResult.GetAwaiter().GetResult();
                } else {
                    throw new InvalidOperationException($"The method {actionName} must return an IActionResult.");
                }
            }

            throw new InvalidOperationException($"The method {actionName} is not found.");
        }

        private MethodInfo? GetMethod(string actionName) {
            Type pageType = GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            MethodInfo? mi = pageType.GetMethod(actionName, flags, null, [typeof(PageHandlerExecutingContext)], null);

            if (mi?.GetParameters()[0].ParameterType == typeof(PageHandlerExecutingContext)) {
                return mi;
            }

            return pageType.GetMethods(flags)
                .FirstOrDefault(method => method.Name == actionName && method.GetParameters().Length == 0);
        }

        private static bool IsAjaxRequest(HttpRequest request) {
            return string.Equals(request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)
                || string.Equals(request.Headers.ContentType, "application/json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
