using System.Reflection;
using Autofac;
using AutoMapper;

namespace CloudyWing.MoneyTrack.Infrastructure.DependencyInjection {
    public class AutoMapperModule : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {
            builder.Register(ctx => new MapperConfiguration(cfg => {
                cfg.AddMaps(new Assembly[] { typeof(AutoMapperModule).Assembly });
            }));

            builder.Register(ctx => {
                MapperConfiguration config = ctx.Resolve<MapperConfiguration>();
                config.AssertConfigurationIsValid();
                return config.CreateMapper();
            }).As<IMapper>()
                .InstancePerLifetimeScope();
        }
    }
}
