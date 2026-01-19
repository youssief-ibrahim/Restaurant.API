using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Restaurant.Application.Interfaces;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private IDbContextTransaction transaction;
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
        public async Task RollbackAsync()
        {
            await transaction.RollbackAsync();
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }
    }
}
