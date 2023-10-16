using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Repositories;
using RocketPDF.Infrastructure.Services;
using System.Reflection;

namespace RocketPDF.EntityFrameworkCore
{
    public class NpgsqlContext : DbContext, IUnitOfWork
    {
        public HttpContext? HttpContext { get; set; }
        public IIdentityService? IdentityService { get; set; }

        public NpgsqlContext(DbContextOptions<NpgsqlContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        private void ProcessEntities()
        {
            var user = IdentityService?.GetUserIdentity();
            foreach (var entry in ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified))
            {
                if (entry.State == EntityState.Added)
                {
                    (entry.Entity as AuditedEntity)?.SetCreatedTime(user?.UserId);
                }
                (entry.Entity as AuditedEntity)?.SetUpdatedTime(user?.UserId);
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProcessEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        public Task ExecuteStrategyAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            return ResilientTransaction.New(this).ExecuteAsync(action, cancellationToken);
        }

        public ValueTask<TResult> ExecuteStrategyAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default)
        {
            return ResilientTransaction.New(this).ExecuteAsync(action, cancellationToken);
        }
    }
}