using Microsoft.Extensions.DependencyInjection;
using PierogiesBot.Data.Services;

namespace PierogiesBot.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            return services.AddTransient(typeof(IRepository<>), typeof(Repository<>))
                .AddSingleton<IMessageBus, MessageBus>();
        }
    }
}