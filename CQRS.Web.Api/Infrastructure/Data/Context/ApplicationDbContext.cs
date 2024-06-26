using CQRS.Web.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace CQRS.Web.Api.Infrastructure.Data.Context
{
    public interface IApplicationDbContext
    {
        public DatabaseFacade Database { get; }
        public ChangeTracker ChangeTracker { get; }
        public DbContext DbContext { get; }

        DbSet<User> Users { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Requestion> Requestions { get; set; }
        DbSet<RequestionDetail> RequestionDetails { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        int SaveChanges();

    }
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private IDbContextTransaction _currentTransaction;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Administrator",
                    Address = "Jakarta",
                    PlaceOfBirth = "Jakarta",
                    BirthOfDate = System.DateTime.Parse("1990-02-01"),
                    isActive = true,
                    Gender = Domain.Enums.Gender.Male,
                    Email = "Administrator@mail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Pa$$w0rd"),
                    CreatedAt = System.DateTime.UtcNow
                }
            );

            modelBuilder.Entity<Requestion>().HasMany(x => x.Details);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Requestion> Requestions { get; set; }
        public DbSet<RequestionDetail> RequestionDetails { get; set; }

        public DbContext DbContext => this;
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
        #region Transaction Handling
        public void BeginTransaction()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            //if (!Database.IsSqlServer())
            //{
            //    _currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
            //}
        }

        public void CommitTransaction()
        {
            try
            {
                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        #endregion

    }
}
