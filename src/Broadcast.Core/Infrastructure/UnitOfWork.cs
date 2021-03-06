﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Broadcast.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Broadcast.Core.Infrastructure
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type)) _repositories[type] = new RepositoryAsync<TEntity>(Context);
            return (IRepositoryAsync<TEntity>) _repositories[type];
        }

        public TContext Context { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}