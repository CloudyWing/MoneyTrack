using System.Web;
using CloudyWing.FormModule.Infrastructure;

namespace CloudyWing.MoneyTrack.Models.Application {
    public class ApplicationService : InfrastructureBase {
        private HttpContextBase httpContext;

        public HttpContextBase HttpContext {
            get {
                if (httpContext is null) {
                    httpContext = GetDependencyService<HttpContextBase>();
                }
                return httpContext;
            }
        }
    }
}
