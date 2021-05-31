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
            services.AddHostedService<DataInitializeHostedService>();
            services.AddHostedService<UserLoginHostedService>();

            services.AddDbContext<AppDbContext>();
            services.AddRestEaseClient<IPierogiesBotApi>(Configuration["APIBaseAddress"]);
            services.AddSingleton(MessageBus.Current);
        }
    }
}