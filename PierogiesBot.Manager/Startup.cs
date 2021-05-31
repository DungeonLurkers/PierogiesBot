using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PierogiesBot.Commons.RestClient;
using PierogiesBot.Manager.Services;
using PierogiesBot.Manager.ViewModels;
using PierogiesBot.Manager.Views;
using ReactiveUI;
using RestEase.HttpClientFactory;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace PierogiesBot.Manager
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPierogiesBotService, PierogiesBotService>();
            services.AddHostedService<UserLoginHostedService>();
            services.AddTransient(typeof(IFactory<>), typeof(Factory<>));

            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<IViewFor<MainWindowViewModel>, MainWindow>();
            services.AddScoped<LoginViewModel>();
            services.AddScoped<IViewFor<LoginViewModel>, LoginView>();

            services.AddRestEaseClient<IPierogiesBotApi>(Configuration["APIBaseAddress"]);
            
            services.AddSingleton(MessageBus.Current);
            
            services.UseMicrosoftDependencyResolver();
            
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();
        }
    }
}