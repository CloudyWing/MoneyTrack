using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using Autofac;
using Autofac.Core;
using CloudyWing.MoneyTrack.Models.Application;
using CloudyWing.MoneyTrack.Models.DataAccess;
using CloudyWing.MoneyTrack.Models.Domain;

namespace CloudyWing.MoneyTrack.App_Start {
    public class ServiceModule : Autofac.Module {
        protected override void Load(ContainerBuilder builder) {
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.StartsWith("CloudyWing.MoneyTrack"));

            builder.Register((c) => SqlClientFactory.Instance)
                .As<DbProviderFactory>()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<DatabaseContext>()
                .AsSelf()
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "connectionString",
                        (pi, ctx) => WebConfigurationManager.ConnectionStrings["Default"].ToString()
                     )
                 )
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWorker>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .Where(x => typeof(DomainService).IsAssignableFrom(x) || typeof(ApplicationService).IsAssignableFrom(x))
                .AsSelf()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }
    }
}
