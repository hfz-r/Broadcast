using System;
using System.Data;
using System.Linq;
using Broadcast.Core.Infrastructure.TypeFinder;
using Broadcast.Data.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Broadcast.Data
{
    public class ApplicationDbContext : DbContext
    {
        private IDbContextTransaction _dbContextTransaction;

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var typeFinder = new AppDomainTypeFinder();
            var typeConfigurations = typeFinder.FindClassesOfType<IMappingConfiguration>().ToList();

            foreach (var typeConfiguration in typeConfigurations)
            {
                var mappingConfiguration = (IMappingConfiguration) Activator.CreateInstance(typeConfiguration);
                mappingConfiguration.ApplyConfiguration(modelBuilder);
            }
        }

        #region Transaction handler

        public void BeginTransaction()
        {
            if (_dbContextTransaction != null)
                return;

            if (!Database.IsInMemory())
                _dbContextTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitTransaction()
        {
            try
            {
                _dbContextTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_dbContextTransaction != null)
                {
                    _dbContextTransaction.Dispose();
                    _dbContextTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _dbContextTransaction?.Rollback();
            }
            finally
            {
                if (_dbContextTransaction != null)
                {
                    _dbContextTransaction.Dispose();
                    _dbContextTransaction = null;
                }
            }
        }

        #endregion
    }
}