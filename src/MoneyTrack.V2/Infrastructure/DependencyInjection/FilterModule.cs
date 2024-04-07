using System;
using System.Linq;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace CloudyWing.MoneyTrack.Infrastructure.DependencyInjection {
    public class FilterModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(x => typeof(FilterAttribute).IsAssignableFrom(x))
                .AsSelf()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();

            builder.RegisterFilterProvider();
        }
    }
}
