using System.Web.Mvc;
using System.Web.Routing;
using CloudyWing.MoneyTrack.Infrastructure.Util;

namespace CloudyWing.MoneyTrack.Areas.Administrators {
    public class AdministratorsAreaRegistration : AreaRegistration {
        public override string AreaName => "Administrators";

        public override void RegisterArea(AreaRegistrationContext context) {
            RouteValueDictionary routeValues = new RouteValueDictionary() {
                ["Action"] = "Index",
                [Constants.UrlParametersKey] = UrlParameter.Optional
            };

            context.MapRoute(
                "Administrators_Default",
                $"Administrators/{{controller}}/{{action}}/{{{Constants.UrlParametersKey}}}",
                routeValues
            );
        }
    }
}
