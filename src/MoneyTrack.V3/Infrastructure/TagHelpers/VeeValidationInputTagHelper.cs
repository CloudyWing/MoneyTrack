using System.ComponentModel.DataAnnotations;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CloudyWing.MoneyTrack.Infrastructure.TagHelpers {
    [HtmlTargetElement("input", Attributes = ForAttributeName)]
    public class VeeValidationInputTagHelper : TagHelper {
        private const string ForAttributeName = "asp-for";
        private const string DataValidationAs = "data-vv-as";
        private const string ValidateAttribute = "v-validate";
        private const string RefAttribute = "ref";
        private const string OtherValidateAttribute = "vee-other-validate";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression? For { get; set; }

        [HtmlAttributeName(OtherValidateAttribute)]
        public string? OtherValidate { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            ExceptionUtils.ThrowIfNull(() => context);
            ExceptionUtils.ThrowIfNull(() => output);

            if (For is null) {
                return;
            }

            if (!context.AllAttributes.ContainsName(DataValidationAs)) {
                output.Attributes.Add(DataValidationAs, For!.Metadata.GetDisplayName());
            }

            if (!context.AllAttributes.ContainsName(RefAttribute)) {
                output.Attributes.Add(RefAttribute, For!.Name);
            }

            if (!context.AllAttributes.ContainsName(ValidateAttribute)) {
                string? validateValues = GetValidateValues();
                if (validateValues != null) {
                    output.Attributes.Add(ValidateAttribute, GetValidateValues());
                }
            }
        }

        private string? GetValidateValues() {
            List<string> items = [];

            if (For is not null) {
                foreach (object validationAttribute in For.Metadata.ValidatorMetadata) {
                    switch (validationAttribute) {
                        case CompareAttribute attr:
                            // HACK 不確定能正確抓到
                            string[] forNameParts = For.Name.Split('.');
                            forNameParts[^1] = attr.OtherProperty;
                            items.Add($"confirmed:{string.Join(".", forNameParts)}");
                            break;
                        case CreditCardAttribute _:
                            items.Add("credit_card");
                            break;
                        case EmailAddressAttribute _:
                            items.Add("email");
                            break;
                        case FileExtensionsAttribute attr:
                            items.Add($"ext:{attr.Extensions}");
                            break;
                        case StringLengthAttribute attr:
                            if (attr.MaximumLength > 0) {
                                items.Add($"max:{attr.MaximumLength}");
                            }
                            if (attr.MinimumLength > 0) {
                                items.Add($"min:{attr.MinimumLength}");
                            }
                            break;
                        case MaxLengthAttribute attr:
                            if (attr.Length > 0) {
                                items.Add($"max:{attr.Length}");
                            }
                            break;
                        case MinLengthAttribute attr:
                            if (attr.Length > 0) {
                                items.Add($"min:{attr.Length}");
                            }
                            break;
                        case PhoneAttribute attr:
                            // UNDONE Vee原生未支援
                            break;
                        case RangeAttribute attr:
                            string key = attr.OperandType == typeof(DateTime)
                                ? "date_between" : "between";
                            items.Add($"{key}:{attr.Minimum},{attr.Maximum}");
                            break;
                        case RegularExpressionAttribute _:
                            // regex只支援object expression的方式，confirmed只支援string expressions的方式
                            // 考量到正規式容易有跳脫問題，所以不在前端驗證正規式
                            break;
                        case RequiredAttribute _:
                            items.Add("required");
                            break;
                        case UrlAttribute _:
                            items.Add("url");
                            break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(OtherValidate)) {
                items.AddRange(OtherValidate.Split('|'));
            }

            if (items.Any()) {
                return $"'{string.Join("|", items)}'";
            }

            return null;
        }
    }
}
