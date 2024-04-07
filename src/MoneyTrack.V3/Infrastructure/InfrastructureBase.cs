using AutoMapper;
using CloudyWing.MoneyTrack.Infrastructure.DependencyInjection;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Microsoft.Extensions.Logging.Abstractions;

namespace CloudyWing.MoneyTrack.Infrastructure {
    /// <summary>
    /// Provides a basic class for infrastructure components.
    /// </summary>
    public abstract class InfrastructureBase {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">The optional service provider used for dependency resolution.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceProvider"/> is <c>null</c>.</exception>
        public InfrastructureBase(IServiceProvider? serviceProvider) {
            ExceptionUtils.ThrowIfNull(() => serviceProvider);

            LazyServiceProvider = LazyServiceProvider = new LazyServiceProvider(serviceProvider);
        }

        /// <summary>
        /// Gets the lazy service provider used for resolving services.
        /// </summary>
        protected LazyServiceProvider LazyServiceProvider { get; }

        /// <summary>
        /// Gets the logger factory obtained from the service provider.
        /// </summary>
        protected ILoggerFactory LoggerFactory => LazyServiceProvider.GetService<ILoggerFactory>();

        /// <summary>
        /// Gets the logger obtained from the service provider.
        /// </summary>
        public ILogger Logger => LazyServiceProvider.GetService<ILogger>(provider => LoggerFactory.CreateLogger(GetType().FullName!) ?? NullLogger.Instance);

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        public IMapper Mapper => LazyServiceProvider.GetService<IMapper>();
    }
}
