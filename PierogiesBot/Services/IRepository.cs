using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PierogiesBot.Models;

namespace PierogiesBot.Services
{
    public interface IRepository<T>
    {
        Task InsertAsync(T doc);
        Task UpdateAsync(T doc);
        Task DeleteAsync(string id);
        Task<T> GetByIdAsync(string id);
        Task<T?> GetByProperty<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value);
        Task<IEnumerable<T>> GetAllByProperty<TProp>(Expression<Func<T, TProp>> propertyAccessor, TProp value);
        Task<IEnumerable<T>> GetByPredicate(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAll();
    }
}