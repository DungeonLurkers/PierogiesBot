using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Persistence.Models;

namespace Persistence.Storage
{
    public class BotResponseRuleMongoDataSource : MongoDataSourceBase<BotResponseRule, Guid>
    {
        private static readonly object WriteLock = new object();
        public BotResponseRuleMongoDataSource(IMongoClient mongoClient, IConfiguration configuration, 
            ILogger<BotResponseRuleMongoDataSource> logger) : base(mongoClient, configuration, WriteLock, logger)
        {
            GetNonMongoIdDelegate = entity => entity.Id;
        }

        public sealed override Expression<Func<BotResponseRule, Guid>> GetNonMongoIdDelegate { get; protected set; }
    }
}