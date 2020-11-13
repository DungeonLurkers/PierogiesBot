// ReSharper disable InconsistentNaming

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Module.Data.Models;
using Module.Data.Storage;
using MongoDB.Driver;
using Persistence.Storage;

namespace Persistence.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(provider =>
            {
                var config = provider.GetService<IConfiguration>();

                var connectionString = config["MongoDb:ConnectionString"];

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ArgumentException("Connection string is empty!");
                }

                return new MongoClient(connectionString);
            });

            services.AddSingleton<IDataSource<RoleEntity, ulong>, RoleMongoDataSource>();
            services.AddSingleton<IDataSource<GuildEntity, ulong>, GuildMongoDataSource>();
            services.AddSingleton<IDataSource<GuildUserEntity, ulong>, GuildUserMongoDataSource>();
            services.AddSingleton<IDataSource<SettingEntity, Guid>, SettingMongoDataSource>();
            services.AddSingleton<IDataSource<BotResponseRule, Guid>, BotResponseRuleMongoDataSource>();
            services.AddSingleton<IDataSource<BotReactRule, Guid>, BotReactRuleMongoDataSource>();
            services.AddSingleton<IDataSource<QuestionEntity, Guid>, QuestionMongoDataSource>();
        }
    }
}