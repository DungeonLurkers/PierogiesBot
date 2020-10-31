using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Module.Data.Models;
using MongoDB.Driver;

namespace Persistence.Storage
{
    public class BotReactRuleMongoDataSource : MongoDataSourceBase<BotReactRule, Guid>
    {
        private static object WriteLock = new object();

        public BotReactRuleMongoDataSource(IMongoClient mongoClient, IConfiguration configuration,
            ILogger<BotReactRuleMongoDataSource> logger) : base(mongoClient, configuration, WriteLock, logger)
        {
            GetNonMongoIdDelegate = rule => rule.Id;
        }

        public sealed override Expression<Func<BotReactRule, Guid>> GetNonMongoIdDelegate { get; protected set; }
    }
}