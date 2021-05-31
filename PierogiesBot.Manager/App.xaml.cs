using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Services;
using PierogiesBot.Manager.ViewModels;
using PierogiesBot.Manager.Views;
using ReactiveUI;

namespace PierogiesBot.Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        private static IHostBuilder DefaultHostBuilder => 
            Host.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureLogging(b => 
                    b.AddDebug()
                     .AddSimpleConsole())
                .UseConsoleLifetime(); 

        public App()
        {
            _host = DefaultHostBuilder.Build();
            Initialize().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task Initialize()
        {
            await _host.StartAsync();

            using var scope = _host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<App>>();
            var pierogiesBotService = scope.ServiceProvider.GetRequiredService<IPierogiesBotService>();
            logger.LogInformation("Starting PierogiesBot Manager");
            try
            {
                Window? mainWindow = null;
                if (pierogiesBotService.IsAuthenticated)
                    mainWindow = scope.ServiceProvider.GetRequiredService<IViewFor<MainWindowViewModel>>() as Window;
                else
                    mainWindow = scope.ServiceProvider.GetRequiredService<IViewFor<LoginViewModel>>() as Window;
                MainWindow = mainWindow;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow?.Show();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            _host.StopAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false).GetAwaiter().GetResult();
            
            _host.Dispose();
        }
    }
}