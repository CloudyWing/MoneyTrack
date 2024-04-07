using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using Autofac;
using Autofac.Core;
using CloudyWing.MoneyTrack.Infrastructure.Options;

namespace CloudyWing.MoneyTrack.DependencyInjection {
    public class OptionsModule : Module {
        protected override void Load(ContainerBuilder builder) {
            RegisterOptions<AccountOptions>(builder);
        }

        private void RegisterOptions<T>(ContainerBuilder builder)
            where T : class {
            string optionsName = typeof(T).Name.Replace("Options", "");
            var registrationBuilder = builder.RegisterType<T>()
                .AsSelf()
                .InstancePerLifetimeScope();

            foreach (string key in WebConfigurationManager.AppSettings.AllKeys.Where(x => x.StartsWith(optionsName))) {
                registrationBuilder.WithParameter(new ResolvedParameter(
                   (pi, ctx) => pi.Name.Equals(
                                    Regex.Replace(key, $@"^{optionsName}:", "", RegexOptions.IgnoreCase),
                                    StringComparison.OrdinalIgnoreCase
                                ),
                   (pi, ctx) => Convert.ChangeType(WebConfigurationManager.AppSettings[key], pi.ParameterType)
                ));
            }
        }
    }
}
