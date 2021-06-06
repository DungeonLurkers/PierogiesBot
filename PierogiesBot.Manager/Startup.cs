using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Commons.RestClient;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using RestEase.HttpClientFactory;

namespace PierogiesBot.Manager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<DataInitializeHostedService>();
            services.AddHostedService<UserLoginHostedService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<AppDbContext>();
            services.AddRestEaseClient<IPierogiesBotApi>(Configuration["APIBaseAddress"]);
            services.AddSingleton(MessageBus.Current);
        }
    }
}