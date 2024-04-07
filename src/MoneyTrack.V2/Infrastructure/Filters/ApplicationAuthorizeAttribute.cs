using System;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.Application.HomeModels;

namespace CloudyWing.MoneyTrack.Infrastructure.Filters {
    public class ApplicationAuthorizeAttribute : AuthorizeAttribute {
        private readonly HomeAppService homeAppService;

        public ApplicationAuthorizeAttribute(HomeAppService homeAppService) {
            ExceptionUtils.ThrowIfNull(() => homeAppService);

            this.homeAppService = homeAppService;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext) {
            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request.IsAjaxRequest()) {
                filterContext.Result = new HttpUnauthorizedResult();
            } else {
                if (request.HttpMethod.Equals(HttpMethod.Get.ToString(), StringComparison.OrdinalIgnoreCase)
                    && homeAppService.TryLoginFromCookie()
                ) {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        ["area"] = filterContext.RouteData.DataTokens["area"],
                        ["controller"] = filterContext.RouteData.Values["controller"],
                        ["action"] = filterContext.RouteData.Values["action"]
                    });
                } else {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                        ["Area"] = "",
                        ["controller"] = "Home",
                        ["action"] = "Logout"
                    });
                }
            }
        }
    }
}
