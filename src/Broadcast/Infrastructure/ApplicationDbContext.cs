using System.Data;
using Broadcast.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Broadcast.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        private IDbContextTransaction _dbContextTransaction;

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<MessageTag> MessageTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageTag>(builder =>
            {
                builder.HasKey(t => new {t.MessageId, t.TagId});

                builder.HasOne(mt => mt.Message)
                    .WithMany(m => m.MessageTags)
                    .HasForeignKey(mt => mt.MessageId);

                builder.HasOne(mt => mt.Tag)
                    .WithMany(m => m.MessageTags)
                    .HasForeignKey(mt => mt.TagId);

                builder.Ignore(mt => mt.Id);
            });
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