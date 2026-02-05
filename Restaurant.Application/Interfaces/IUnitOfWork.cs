using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Models;

namespace Restaurant.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChanges();
        IGenericRebosatory<T> Genunit<T>() where T : class;
        //IGenericRebosatory<Meal> MealRepo { get; }

    }
}
