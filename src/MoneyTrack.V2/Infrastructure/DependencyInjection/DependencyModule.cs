using System;
using System.Linq;
using System.Reflection;
using Autofac;

namespace CloudyWing.MoneyTrack.Infrastructure.DependencyInjection {
    public class DependencyModule : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.StartsWith("CloudyWing.MoneyTrack"))
                .ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(x => typeof(ITransientDependency).IsAssignableFrom(x))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(x => typeof(IScopedDependency).IsAssignableFrom(x))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterAssemblyTypes(assemblies)
                .Where(x => typeof(ISingleDependency).IsAssignableFrom(x))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }
    }
}
