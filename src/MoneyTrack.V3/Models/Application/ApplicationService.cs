using CloudyWing.MoneyTrack.Infrastructure;
using CloudyWing.MoneyTrack.Infrastructure.DependencyInjection;

namespace CloudyWing.MoneyTrack.Models.Application {
    /// <summary>
    /// Represents a service message provider.
    /// </summary>
    public abstract class ApplicationService : InfrastructureBase, IScopedDependency {
        private HttpContext? httpContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        protected ApplicationService(IServiceProvider? serviceProvider) : base(serviceProvider) { }

        private IHttpContextAccessor HttpContextAccessor => LazyServiceProvider.GetService<IHttpContextAccessor>();

        /// <summary>
        /// Gets the current HttpContext.
        /// </summary>
        protected HttpContext HttpContext {
            get {
                httpContext ??= HttpContextAccessor.HttpContext!;
                return httpContext;
            }
        }
    }
}
