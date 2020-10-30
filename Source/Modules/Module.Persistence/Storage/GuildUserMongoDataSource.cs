using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Persistence.Models;

namespace Persistence.Storage
{
    public class GuildUserMongoDataSource : MongoDataSourceBase<GuildUserEntity, ulong>
    {
        private static readonly object WriteLock = new object();
        public GuildUserMongoDataSource(IMongoClient mongoClient, 
            IConfiguration configuration, 
            ILogger<GuildUserMongoDataSource> logger) : base(mongoClient, configuration, WriteLock, logger)
        {
            GetNonMongoIdDelegate = entity => entity.DiscordId;
        }

        public sealed override Expression<Func<GuildUserEntity, ulong>> GetNonMongoIdDelegate { get; protected set; }
    }
}