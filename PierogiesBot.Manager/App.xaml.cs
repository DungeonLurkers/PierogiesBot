using System;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using PierogiesBot.Manager.Services;
using PierogiesBot.Manager.ViewModels;
using ReactiveUI;
using Splat.Microsoft.Extensions.Logging;

namespace PierogiesBot.Manager
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        private ILogger<App>? _logger;

        public App()
        {
            _host = DefaultHostBuilder.Build();

            InitializeAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            Container = _host.Services;
        }

        public static IServiceProvider Container { get; private set; }

        private static IHostBuilder DefaultHostBuilder =>
            Host.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.RegisterModule<AutofacModule>();
                }).ConfigureLogging(b =>
                    b.ClearProviders()
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddNLog("NLog.config"))
                .UseConsoleLifetime();

        public async Task InitializeAsync()
        {
            await _host.StartAsync();

            using var scope = _host.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await dbContext.Database.MigrateAsync();

            _logger = scope.ServiceProvider.GetRequiredService<ILogger<App>>();
            _logger.LogInformation("Starting PierogiesBot Manager");
            try
            {
                _logger.LogTrace("Constructing new main window");
                MainWindow = scope.ServiceProvider.GetRequiredService<IViewFor<MainWindowViewModel>>() as Window;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            _logger.LogTrace("Showing main window");
            MainWindow?.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            _host.StopAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false).GetAwaiter().GetResult();

            _host.Dispose();
        }
    }
}