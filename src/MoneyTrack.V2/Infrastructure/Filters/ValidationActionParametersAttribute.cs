using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace CloudyWing.MoneyTrack.Infrastructure.Filters {
    /// <remarks>
    /// ref: https://blog.markvincze.com/how-to-validate-action-parameters-with-dataannotation-attributes/
    /// </remarks>
    public sealed class ValidationActionParametersAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext context) {
            ParameterDescriptor[] parameters = context.ActionDescriptor.GetParameters();

            foreach (var parameter in parameters) {
                object argument = context.ActionParameters[parameter.ParameterName];

                EvaluateValidationAttributes(parameter, argument, context.Controller.ViewData.ModelState);
            }

            base.OnActionExecuting(context);
        }

        private void EvaluateValidationAttributes(ParameterDescriptor parameter, object argument, ModelStateDictionary modelState) {
            IEnumerable<ValidationAttribute> validationAttributes = parameter
                .GetCustomAttributes(typeof(ValidationAttribute), true)
                .OfType<ValidationAttribute>();

            foreach (ValidationAttribute validationAttribute in validationAttributes) {
                bool isValid = validationAttribute.IsValid(argument);
                if (!isValid) {
                    modelState.AddModelError(parameter.ParameterName, validationAttribute.FormatErrorMessage(parameter.ParameterName));
                }
            }
        }
    }
}
