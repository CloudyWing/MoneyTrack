using System.Reflection;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using CloudyWing.MoneyTrack.Models.DataAccess;
using Microsoft.Data.SqlClient;

namespace CloudyWing.MoneyTrack.Infrastructure.DependencyInjection {
    public static class ServicesExtensions {
        public static IServiceCollection AddDependencies(this IServiceCollection services, string? connectionString) {
            ExceptionUtils.ThrowIfNullOrWhiteSpace(() => connectionString);

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            foreach (Type type in executingAssembly.GetTypes()) {
                if (!type.IsAbstract && !type.IsInterface) {
                    if (type.IsAssignableTo(typeof(IScopedDependency))) {
                        RegisterServices(services, type, ServiceLifetime.Scoped);
                    }

                    if (type.IsAssignableTo(typeof(ITransientDependency))) {
                        RegisterServices(services, type, ServiceLifetime.Transient);
                    }
                }
            }

            services.AddScoped((x) => new DatabaseContext(SqlClientFactory.Instance, connectionString));
            services.AddScoped<UnitOfWorker>();

            return services;
        }

        private static void RegisterServices(IServiceCollection services, Type concreteType, ServiceLifetime lifetime) {
            services.Add(new ServiceDescriptor(concreteType, concreteType, lifetime));

            Type? baseType = concreteType.BaseType;
            while (baseType is not null) {
                services.Add(new ServiceDescriptor(baseType, concreteType, lifetime));
                baseType = baseType.BaseType;
            }

            Type[] interfaceTypes = concreteType.GetInterfaces();
            foreach (Type interfaceType in interfaceTypes) {
                services.Add(new ServiceDescriptor(interfaceType, concreteType, lifetime));
            }
        }
    }
}
