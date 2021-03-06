﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Core.Domain;
using Broadcast.Core.Infrastructure.Paging;
using Microsoft.EntityFrameworkCore.Query;

namespace Broadcast.Core.Infrastructure
{
    public interface IRepositoryAsync<T> : IDisposable where T : BaseEntity
    {
        Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));
        Task AddAsync(params T[] entities);
        Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
        void Delete(IEnumerable<T> entities);
        Task DeleteAsync(object id);
        void Delete(params T[] entities);
        Task DeleteAsync(T entity);
        Task<IPaginate<TResult>> GetPagedListAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, int index = 0, int size = int.MaxValue, bool disableTracking = false, CancellationToken cancellationToken = default(CancellationToken)) where TResult : class;
        Task<IPaginate<T>> GetPagedListAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, int index = 0, int size = int.MaxValue, bool disableTracking = false, CancellationToken cancellationToken = default(CancellationToken));
        Task<IQueryable<T>> GetQueryableAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IQueryable<T>> queryExp = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, bool disableTracking = false);
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = false);
        void Update(IEnumerable<T> entities);
        void Update(params T[] entities);
        void Update(T entity);
    }
}