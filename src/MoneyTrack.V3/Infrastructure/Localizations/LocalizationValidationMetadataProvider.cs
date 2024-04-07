using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace CloudyWing.MoneyTrack.Infrastructure.Localizations {
    public class LocalizationValidationMetadataProvider : IValidationMetadataProvider {
        private readonly ResourceManager resourceManager;
        private readonly Type resourceType;

        public LocalizationValidationMetadataProvider(Type type) {
            ExceptionUtils.ThrowIfNull(() => type);

            resourceType = type;
            resourceManager = new ResourceManager(type);
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context) {
            foreach (ValidationAttribute attribute in context.ValidationMetadata.ValidatorMetadata.OfType<ValidationAttribute>()) {
                if (attribute.ErrorMessageResourceName is null) {
                    bool hasErrorMessage = attribute.ErrorMessage != null;

                    if (hasErrorMessage) {
                        string? defaultErrorMessage = typeof(ValidationAttribute)
                            .GetField("_defaultErrorMessage", BindingFlags.NonPublic | BindingFlags.Instance)
                            ?.GetValue(attribute) as string;

                        // 部分ValidationAttribute的ErrorMessage預設不為Null
                        hasErrorMessage = attribute.ErrorMessage != defaultErrorMessage;
                    }

                    if (hasErrorMessage) {
                        continue;
                    }

                    string? name = GetMessageName(attribute);
                    if (name != null && resourceManager.GetString(name) != null) {
                        attribute.ErrorMessageResourceType = resourceType;
                        attribute.ErrorMessageResourceName = name;
                        attribute.ErrorMessage = null;
                    }
                }
            }
        }

        private string? GetMessageName(ValidationAttribute attr) {
            switch (attr) {
                case CompareAttribute _:
                    return "CompareAttribute_MustMatch";
                case StringLengthAttribute vAttr:
                    if (vAttr.MinimumLength > 0) {
                        return "StringLengthAttribute_ValidationErrorIncludingMinimum";
                    }
                    return "StringLengthAttribute_ValidationError";
                case DataTypeAttribute _:
                    return $"{attr.GetType().Name}_Invalid";
                case ValidationAttribute _:
                    return $"{attr.GetType().Name}_ValidationError";
            }

            return null;
        }
    }
}
