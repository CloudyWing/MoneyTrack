using CloudyWing.MoneyTrack.Infrastructure.Util;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CloudyWing.MoneyTrack.Infrastructure.TagHelpers {
    [HtmlTargetElement("span", Attributes = ValidationForAttributeName)]
    public class VeeValidationMessageTagHelper : TagHelper {
        private const string ValidationForAttributeName = "vee-validation-for";
        private const string VueShow = "v-show";

        [HtmlAttributeName(ValidationForAttributeName)]
        public ModelExpression? For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            ExceptionUtils.ThrowIfNull(() => context);
            ExceptionUtils.ThrowIfNull(() => output);

            if (For is null) {
                return;
            }

            output.Attributes.Add(VueShow, $"errors.has('{For.Name}')");
            output.Content.SetHtmlContent($"{{{{ errors.first('{For.Name}') }}}}");
        }
    }
}
