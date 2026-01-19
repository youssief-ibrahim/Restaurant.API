using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IGenericRebosatory<T> where T : class
    {
        Task<List<T>> GetAll(params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllwithsearch(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes);
        Task<T> GetById(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<List<T>> FindAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> checkId(int id);
        Task Create(T item);
        void update(T item);
        void delete(T item);
    }
}
