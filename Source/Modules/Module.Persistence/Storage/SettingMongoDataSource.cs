using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Persistence.Models;

namespace Persistence.Storage
{
    public class SettingMongoDataSource : MongoDataSourceBase<SettingEntity, Guid>
    {
        private static readonly object WriteLock = new object();
        public SettingMongoDataSource(IMongoClient mongoClient, 
            ILogger<SettingMongoDataSource> logger,
            IConfiguration configuration) : base(mongoClient, configuration, WriteLock, logger)
        {
            GetNonMongoIdDelegate = entity => entity.Id;
        }

        public sealed override Expression<Func<SettingEntity, Guid>> GetNonMongoIdDelegate { get; protected set; }
    }
}