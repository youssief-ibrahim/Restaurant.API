using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Models;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private IDbContextTransaction transaction;
        //private IGenericRebosatory<Meal> mealRepo;
        private readonly Dictionary<Type, object> repositories = new();
        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task BeginTransactionAsync()
        {
            transaction = await context.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            await transaction.CommitAsync();
        }

        public IGenericRebosatory<T> Genunit<T>() where T : class
        {
            var typename = typeof(T);
            if(repositories.ContainsKey(typename))
            {
                return (IGenericRebosatory<T>)repositories[typename];
            }
            var repository = new GenericRebosatory<T>(context);
            repositories[typename] = repository;
            return repository;
        }

        public async Task RollbackAsync()
        {
            await transaction.RollbackAsync();
        }
        //public IGenericRebosatory<Meal> Genunit {

        //    get
        //    {
        //        if (mealRepo==null)
        //        {
        //            mealRepo = new GenericRebosatory<Meal>(context);
        //        }
        //        return mealRepo;
        //    }
        //}


        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }
    }
}
