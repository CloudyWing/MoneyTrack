using System.IO;
using System.Windows;
using CloudyWing.MoneyTrack.Tools.EntityGenerator.Options;
using CloudyWing.MoneyTrack.Tools.EntityGenerator.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudyWing.MoneyTrack.Tools.EntityGenerator {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            ServiceCollection serviceCollection = new();
            ConfigureServices(serviceCollection, configuration);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>()!;
            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration) {
            services.Configure<AppOptions>(configuration!.GetSection(AppOptions.Key));

            services.AddTransient<MainWindow>();
            services.AddTransient<SchemaPage>();
            services.AddTransient<SqlPage>();
            services.AddTransient<SchemaPageViewModel>();
            services.AddTransient<SqlPageViewModel>();
        }
    }
}
