using CloudyWing.MoneyTrack.Infrastructure;
using CloudyWing.MoneyTrack.Infrastructure.DependencyInjection;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;

namespace CloudyWing.MoneyTrack.Models.Domain {
    public abstract class DomainService : InfrastructureBase, IScopedDependency {
        protected DomainService(UnitOfWorker? unitOfWorker, IServiceProvider? serviceProvider) : base(serviceProvider) {
            ExceptionUtils.ThrowIfNull(() => unitOfWorker);

            UnitOfWorker = unitOfWorker;
        }

        protected UnitOfWorker UnitOfWorker { get; }
    }
}
