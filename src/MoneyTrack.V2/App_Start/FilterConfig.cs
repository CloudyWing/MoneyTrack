using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Filters;

namespace MoneyTrack.V2 {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new NoCacheAttribute());
            filters.Add(new ValidationActionParametersAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(DependencyResolver.Current.GetService<ApplicationAuthorizeAttribute>());
        }
    }
}
