using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Module.Data.Models;
using MongoDB.Driver;

namespace Persistence.Storage
{
    public class QuestionMongoDataSource : MongoDataSourceBase<QuestionEntity, Guid>
    {
        private static readonly object WriteLock = new object();
        public QuestionMongoDataSource(IMongoClient mongoClient, 
            IConfiguration configuration,
            ILogger<QuestionMongoDataSource> logger) : base(mongoClient, configuration, WriteLock, logger)
        {
            GetNonMongoIdDelegate = entity => entity.Id;
        }

        public sealed override Expression<Func<QuestionEntity, Guid>> GetNonMongoIdDelegate { get; protected set; }
    }
}