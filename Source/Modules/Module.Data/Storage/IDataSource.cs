using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace Module.Data.Storage
{
    public interface IDataSource<T, in TId> where T : class
    {
        void AddOrUpdate(T entity);
        void AddRange(IEnumerable<T> entities);
        void AddOrUpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        T? Get(TId id);
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
    }
}