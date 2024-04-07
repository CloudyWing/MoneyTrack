using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using CloudyWing.MoneyTrack.DependencyInjection;
using CloudyWing.MoneyTrack.Infrastructure.DependencyInjection;

namespace CloudyWing.MoneyTrack.App_Start {
    public static class DependencyConfig {
        public static void RegisterResolver() {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterModule<AutofacWebTypesModule>();

            builder.RegisterModule<DependencyModule>();
            builder.RegisterModule<OptionsModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<AutoMapperModule>();
            builder.RegisterModule<FilterModule>();

            // 建立容器
            IContainer container = builder.Build();

            // 解析容器內的型別
            AutofacDependencyResolver resolver = new AutofacDependencyResolver(container);

            // 建立相依解析器
            DependencyResolver.SetResolver(resolver);
        }
    }
}
