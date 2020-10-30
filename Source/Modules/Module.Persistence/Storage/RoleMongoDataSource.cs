using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Persistence.Models;

namespace Persistence.Storage
{
    public class RoleMongoDataSource : MongoDataSourceBase<RoleEntity, ulong>
    {
        private static readonly object WriteLock = new object();
        public RoleMongoDataSource(IMongoClient mongoClient, 
            IConfiguration configuration, 
            ILogger<RoleMongoDataSource> logger) : base(mongoClient, configuration, WriteLock, logger)
        {
            GetNonMongoIdDelegate = role => role.DiscordId;
        }

        public sealed override Expression<Func<RoleEntity, ulong>> GetNonMongoIdDelegate { get; protected set; }
    }
}