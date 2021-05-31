﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PierogiesBot.Manager.Services;
using PierogiesBot.Manager.ViewModels;
using PierogiesBot.Manager.Views;
using ReactiveUI;
using Splat.Microsoft.Extensions.Logging;

namespace PierogiesBot.Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Container { get; private set; }
        private IHost _host;

        private static IHostBuilder DefaultHostBuilder => 
            Host.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((context, builder) =>
                {
                    builder.RegisterModule<AutofacModule>();
                })
                .ConfigureLogging(b => 
                    b.AddDebug()
                     .AddSimpleConsole()
                     .AddSplat())
                .UseConsoleLifetime(); 

        public App()
        {
            _host = DefaultHostBuilder.Build();
            Initialize().ConfigureAwait(false).GetAwaiter().GetResult();
            
            Container = _host.Services;;
        }

        public async Task Initialize()
        {
            await _host.StartAsync();

            using var scope = _host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<App>>();
            logger.LogInformation("Starting PierogiesBot Manager");
            try
            {
                MainWindow = scope.ServiceProvider.GetRequiredService<IViewFor<MainWindowViewModel>>() as Window;;
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