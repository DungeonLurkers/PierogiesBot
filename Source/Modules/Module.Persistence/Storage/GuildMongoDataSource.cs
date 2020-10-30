using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Persistence.Models;

namespace Persistence.Storage
{
    public class GuildMongoDataSource : MongoDataSourceBase<GuildEntity, ulong>
    {
        private static readonly object WriteLock = new object();
        public GuildMongoDataSource(IMongoClient mongoClient, 
            IConfiguration configuration, 
            ILogger<GuildMongoDataSource> logger) : base(mongoClient, configuration, WriteLock, logger)
        {
            GetNonMongoIdDelegate = entity => entity.DiscordId;
        }

        public sealed override Expression<Func<GuildEntity, ulong>> GetNonMongoIdDelegate { get; protected set; }
    }
}