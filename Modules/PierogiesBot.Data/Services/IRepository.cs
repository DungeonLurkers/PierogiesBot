using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PierogiesBot.Data.Services
{
    public interface IRepository<T>
    {
        Task<string> InsertAsync(T doc);
        Task UpdateAsync(T doc);
        Task DeleteAsync(string id);
        Task<T> GetByIdAsync(string id);
        Task<T?> GetByProperty<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value);
        Task<IEnumerable<T>> GetAllByProperty<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value);
        Task<IEnumerable<T>> GetByPredicate(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAll();
    }
}