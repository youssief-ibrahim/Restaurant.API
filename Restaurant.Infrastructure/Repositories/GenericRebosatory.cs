using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Repositories
{
    public class GenericRebosatory<T> : IGenericRebosatory<T> where T : class
    {
        private readonly ApplicationDbContext context;
        public GenericRebosatory(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>();
            if (query != null)
            {
                foreach (var item in includes)
                    query = query.Include(item);
            }
            return query;

        }
        public async Task<List<T>> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = GetQueryable(includes);
            var res = await query.ToListAsync();
            return res;
        }
        public async Task<List<T>> GetAllwithsearch(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes)
        {
            var query = GetQueryable(includes).Where(filter);
            var res = await query.ToListAsync();
            return res;
        }
        public async Task<T> GetById(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = GetQueryable(includes);
            var res = await query.FirstOrDefaultAsync(predicate);
            return res;
        }
        public async Task<T> checkId(int id)
        {
            var res = await context.Set<T>().FindAsync(id);
            return res;
        }
        public async Task<List<T>> FindAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var quary = GetQueryable(includes).Where(predicate);
            return await quary.ToListAsync();
        }
        public async Task Create(T item)
        {
            await context.Set<T>().AddAsync(item);
        }
        public void update(T item)
        {
            context.Set<T>().Update(item);
        }
        public void delete(T item)
        {
            context.Set<T>().Remove(item);
        }

    }
}
