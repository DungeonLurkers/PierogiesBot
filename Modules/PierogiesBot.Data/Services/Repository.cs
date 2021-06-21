using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Models.EntityChanged;

namespace PierogiesBot.Data.Services
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        protected const string Database = "PierogiesBot";
        private readonly IMongoClient _client;

        private readonly string _collection = typeof(T).Name;
        private readonly ILogger<Repository<T>> _logger;
        private readonly IMessageBus _messageBus;

        public Repository(IMongoClient client, ILogger<Repository<T>> logger, IMessageBus messageBus)
        {
            _client = client;
            _logger = logger;
            _messageBus = messageBus;

            if (!_client.GetDatabase(Database).ListCollectionNames().ToList().Contains(_collection))
                _client.GetDatabase(Database).CreateCollection(_collection);
        }

        protected virtual IMongoCollection<T> Collection =>
            _client.GetDatabase(Database).GetCollection<T>(_collection);

        public async Task<string> InsertAsync(T doc)
        {
            _logger.LogTrace("{0}: Doc Id = {1} of type {2}", nameof(InsertAsync), doc.Id, typeof(T).Name);
            await Collection.InsertOneAsync(doc);
            _messageBus.SendEntityChanged(new AddEntity<T>(doc));
            return doc.Id;
        }

        public async Task UpdateAsync(T doc)
        {
            _logger.LogTrace("{0}: Doc Id = {1} of type {2}", nameof(UpdateAsync), doc.Id, typeof(T).Name);
            Expression<Func<T, string>> func = f => f.Id;
            var value = (string) doc.GetType().GetProperty(func.Body.ToString().Split(".")[1]).GetValue(doc, null);
            if (func is not null)
            {
                var filter = Builders<T>.Filter.Eq(func!, value);

                await Collection.ReplaceOneAsync(filter, doc);
                _messageBus.SendEntityChanged(new UpdateEntity<T>(doc));
            }
        }

        public async Task DeleteAsync(string id)
        {
            _logger.LogTrace("{0}: Doc Id = {1} of type {2}", nameof(UpdateAsync), id, typeof(T).Name);
            await Collection.DeleteOneAsync(f => f.Id == id);
            _messageBus.SendEntityChanged(new RemoveEntity<T>(id));
        }

        public async Task<T> GetByIdAsync(string id)
        {
            _logger.LogTrace("{0}: id = {1} of type {2}", nameof(GetByIdAsync), id, typeof(T).Name);
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T?> GetByProperty<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value)
        {
            _logger.LogTrace("{0}: Searching entity for property of type {1} and value {2} of type {3}",
                nameof(GetByProperty), typeof(T).Name, value, typeof(T).Name);
            var filter = Builders<T>.Filter.Eq(propertyAccessor, value);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllByProperty<TProp>(Expression<Func<T, TProp>> propertyAccessor,
            TProp value)
        {
            _logger.LogTrace("{0}: Searching entity for property of type {1} and value {2} of type {3}",
                nameof(GetByProperty), typeof(T).Name, value, typeof(T).Name);
            var filter = Builders<T>.Filter.Eq(propertyAccessor, value);
            return await Collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByPredicate(
            Expression<Func<T, bool>> predicate)
        {
            _logger.LogTrace("{0} of {1}", nameof(GetByPredicate), typeof(T).Name);
            var filter = Builders<T>.Filter.Where(predicate);
            return await Collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            _logger.LogTrace("{0} of {1}", nameof(GetAll), typeof(T).Name);
            return await Collection.AsQueryable().ToListAsync();
        }
    }
}