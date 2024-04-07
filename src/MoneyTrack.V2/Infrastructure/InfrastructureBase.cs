using System.Web.Mvc;
using CloudyWing.MoneyTrack.Infrastructure.Hosting;

namespace CloudyWing.FormModule.Infrastructure {
    /// <summary>
    /// Provides a basic class for infrastructure components.
    /// </summary>
    public class InfrastructureBase {
        private HostEnvironment hostEnvironment;

        public HostEnvironment HostEnvironment {
            get {
                if (hostEnvironment is null) {
                    hostEnvironment = GetDependencyService<HostEnvironment>();
                }
                return hostEnvironment;
            }
        }

        public TService GetDependencyService<TService>() {
            return DependencyResolver.Current.GetService<TService>();
        }
    }
}
