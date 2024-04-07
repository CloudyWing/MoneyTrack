using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CloudyWing.MoneyTrack.App_Start;
using CloudyWing.MoneyTrack.Infrastructure.ModelBinding;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models;

namespace MoneyTrack.V2 {
    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            DependencyConfig.RegisterResolver();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngineConfig.RegisterViewEngines(ViewEngines.Engines);

            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.Single(x => x.GetType() == typeof(FormValueProviderFactory)));
            ValueProviderFactories.Factories.Add(new EncryptedQueryStringValueProviderFactory());

            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.Single(x => x.GetType() == typeof(RouteDataValueProviderFactory)));
            ValueProviderFactories.Factories.Add(new EncryptedRouteDataValueProviderFactory());

            ModelBinderProviders.BinderProviders.Add(new EnumerationModelBinderProvider());
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e) {
            if (HttpContext.Current != null) {
                UserContext user = new UserContext();

                if (user.Load(new HttpContextWrapper(HttpContext.Current)) && user.UserId != null) {
                    HttpContext.Current.User = new GenericPrincipal(
                        new GenericIdentity(user.UserId),
                        new string[] { }
                    );
                }
            }
        }

        public override string GetVaryByCustomString(HttpContext context, string custom) {
            const string OutputCacheKey = "OutputCacheKey";

            if (context.User.Identity.IsAuthenticated && custom.Equals("User", StringComparison.OrdinalIgnoreCase)) {
                if (Session[OutputCacheKey] is null
                    || !(Session[OutputCacheKey] is VaryByCustomInfo customInfo)
                    || customInfo.UserId == context.User.Identity.Name) {
                    Guid value = Guid.NewGuid();
                    Session[OutputCacheKey] = new VaryByCustomInfo(context.User.Identity.Name, value);
                    return value.ToString();
                }

                return customInfo.Value.ToString();
            }
            return base.GetVaryByCustomString(context, custom);
        }

        private class VaryByCustomInfo {
            public VaryByCustomInfo(string userId, Guid value) {
                ExceptionUtils.ThrowIfNull(() => userId);

                UserId = userId;
                Value = value;
            }

            public string UserId { get; }

            public Guid Value { get; }
        }
    }
}
