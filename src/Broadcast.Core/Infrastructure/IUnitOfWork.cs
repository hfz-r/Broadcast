using System;
using System.Threading.Tasks;
using Broadcast.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Broadcast.Core.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : BaseEntity;

        Task<int> SaveChangesAsync();
    }

    public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}