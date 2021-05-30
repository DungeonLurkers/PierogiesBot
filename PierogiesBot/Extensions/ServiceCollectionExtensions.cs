using AspNetCore.Identity.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PierogiesBot.Data.Services;
using PierogiesBot.Settings;

namespace PierogiesBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services) =>
            services.AddSingleton<IMongoClient>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<MongoDBOption>>();

                return new MongoClient(settings.Value.ConnectionString);
            });
    }
}