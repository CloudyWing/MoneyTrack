using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace CloudyWing.MoneyTrack {
    public static class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            FriendlyUrlSettings settings = new FriendlyUrlSettings {
                AutoRedirectMode = RedirectMode.Permanent
            };
            routes.EnableFriendlyUrls(settings);
        }
    }
}
