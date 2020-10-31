using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Module.Data.Models;
using Module.Data.Storage;
using MongoDB.Bson;
using MongoDB.Driver;
using Persistence.Helpers;

// ReSharper disable InconsistentlySynchronizedField

namespace Persistence.Storage
{
    public abstract class MongoDataSourceBase<TEntity, TId> : IDataSource<TEntity, TId> where TEntity : EntityBase<TId>
    {
        private readonly ILogger<MongoDataSourceBase<TEntity, TId>> _logger;
        private object WriteLock { get; }
        private readonly IMongoCollection<TEntity> _mongoCollection;

        public MongoDataSourceBase(IMongoClient mongoClient, IConfiguration configuration, object writeLock, ILogger<MongoDataSourceBase<TEntity, TId>> logger)
        {
            _logger = logger;
            WriteLock = writeLock;

            var dbName = configuration["MongoDb:DatabaseName"];
            if(string.IsNullOrWhiteSpace(dbName))
                throw new ArgumentNullException(nameof(configuration), "MongoDb name not found in configuration!");

            var db = mongoClient.GetDatabase(dbName);

            if (db == null)
            {
                throw new MongoException("Database not found or error!");
            }

            var collName = MongoHelper.GetCollNameForEntityType(typeof(TEntity));
                
            _mongoCollection = db.GetCollection<TEntity>(collName);

            if (_mongoCollection != null) return;
            _logger.LogDebug("Collection {0} not found! Trying to create...", collName);
            db.CreateCollection(collName);

            _mongoCollection = db.GetCollection<TEntity>(collName);
            _logger.LogDebug("Collection {0} created!", _mongoCollection.CollectionNamespace);
        }

        public abstract Expression<Func<TEntity, TId>> GetNonMongoIdDelegate { get; protected set; }

        public virtual void AddOrUpdate(TEntity entity)
        {
            lock (WriteLock)
            {
                var getId = GetNonMongoIdDelegate.Compile();

                if (_mongoCollection.FindSync(Builders<TEntity>.Filter.Eq(GetNonMongoIdDelegate, getId(entity))).Any())
                {
                    _mongoCollection
                        .FindOneAndReplace(
                            Builders<TEntity>.Filter.Eq(GetNonMongoIdDelegate, getId(entity)), entity);
                }
                else
                {
                    _mongoCollection.InsertOne(entity);
                }
            }
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            lock (WriteLock) _mongoCollection.InsertMany(entities);
        }

        public virtual void AddOrUpdateRange(IEnumerable<TEntity> entities)
        {
            lock (WriteLock)
            {
                var getId = GetNonMongoIdDelegate.Compile();
                foreach (var entity in entities)
                {
                    if (_mongoCollection.FindSync(Builders<TEntity>.Filter.Eq(GetNonMongoIdDelegate, getId(entity))).Any())
                    {
                        _mongoCollection
                            .FindOneAndReplace(
                                Builders<TEntity>.Filter.Eq(GetNonMongoIdDelegate, getId(entity)), entity);
                    }
                    else
                    {
                        _mongoCollection.InsertOne(entity);
                    }
                }
            }
        }

        public virtual void Remove(TEntity entity)
        {
            var getId = GetNonMongoIdDelegate.Compile();
            lock (WriteLock)
                _mongoCollection.DeleteOne(Builders<TEntity>.Filter.Eq(GetNonMongoIdDelegate, getId(entity)));
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            var getId = GetNonMongoIdDelegate.Compile();
            var ids = entities.Select(getId).ToList();

            lock (WriteLock)
                _mongoCollection.DeleteMany(Builders<TEntity>.Filter.Where(entity => ids.Contains(getId(entity))));
        }

        public virtual TEntity? Get(TId id) =>
            _mongoCollection
                .FindSync(Builders<TEntity>.Filter.Eq(GetNonMongoIdDelegate, id))
                .Single();

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate) => _mongoCollection
            .FindSync(Builders<TEntity>.Filter.Where(predicate))
            .ToList();

        public virtual IEnumerable<TEntity> GetAll() => _mongoCollection.FindSync(FilterDefinition<TEntity>.Empty).ToList();
    }
}