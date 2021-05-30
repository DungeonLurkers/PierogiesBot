using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PierogiesBot.Data.Models;
using PierogiesBot.Data.Models.EntityChanged;

namespace PierogiesBot.Data.Services
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        protected const string Database = "PierogiesBot";
        private readonly IMediator _mediator;
        private readonly IMongoClient _client;
        private readonly ILogger<Repository<T>> _logger;

        private readonly string _collection = typeof(T).Name;

        public Repository(IMediator mediator, IMongoClient client, ILogger<Repository<T>> logger)
        {
            _mediator = mediator;
            _client = client;
            _logger = logger;

            if (!_client.GetDatabase(Database).ListCollectionNames().ToList().Contains(_collection))
                _client.GetDatabase(Database).CreateCollection(_collection);
        }

        protected virtual IMongoCollection<T> Collection =>
            _client.GetDatabase(Database).GetCollection<T>(_collection);

        public async Task InsertAsync(T doc)
        {
            _logger.LogTrace("{0}: Doc Id = {1}", nameof(InsertAsync), doc.Id);
            await Collection.InsertOneAsync(doc);
            await _mediator.Publish(new AddEntity<T>(doc));
        }

        public async Task UpdateAsync(T doc)
        {
            _logger.LogTrace("{0}: Doc Id = {1}", nameof(UpdateAsync), doc.Id);
            Expression<Func<T, string>> func = f => f.Id;
            var value = (string) doc.GetType().GetProperty(func.Body.ToString().Split(".")[1]).GetValue(doc, null);
            if (func is not null)
            {
                var filter = Builders<T>.Filter.Eq(func!, value);

                await Collection.ReplaceOneAsync(filter, doc);
                await _mediator.Publish(new UpdateEntity<T>(doc));
            }
        }

        public async Task DeleteAsync(string id)
        {
            _logger.LogTrace("{0}: Doc Id = {1}", nameof(UpdateAsync), id);
            await Collection.DeleteOneAsync(f => f.Id == id);
            await _mediator.Publish(new RemoveEntity<T>(id));
        }

        public async Task<T> GetByIdAsync(string id)
        {
            _logger.LogTrace("{0}: id = {1}", nameof(GetByIdAsync), id);
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }
        
        public async Task<T?> GetByProperty<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value)
        {
            _logger.LogTrace("{0}: Searching entity for property of type {1} and value {2}", nameof(GetByProperty), typeof(T).Name, value);
            var filter = Builders<T>.Filter.Eq(propertyAccessor, value);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }
        
        public async Task<IEnumerable<T>> GetAllByProperty<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value)
        {
            _logger.LogTrace("{0}: Searching entity for property of type {1} and value {2}", nameof(GetByProperty), typeof(T).Name, value);
            var filter = Builders<T>.Filter.Eq(propertyAccessor, value);
            return await Collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByPredicate(
            Expression<Func<T, bool>> predicate)
        {
            _logger.LogTrace("{0}", nameof(GetByPredicate));
            var filter = Builders<T>.Filter.Where(predicate);
            return await Collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await Collection.AsQueryable().ToListAsync();
        }
    }
}