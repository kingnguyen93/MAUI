using Microsoft.EntityFrameworkCore;
using RocketPDF.Domain.Entities;
using RocketPDF.Infrastructure.Repositories;

namespace RocketPDF.EntityFrameworkCore.Repositories
{
    public class Repository<TEntity, TKey> : Repository<TEntity>, IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
        where TKey : IComparable<TKey>
    {
        public Repository(NpgsqlContext context) : base(context)
        {
        }

        public TEntity? Get(TKey id)
        {
            return DbSet.Where(x => x.Id.Equals(id)).FirstOrDefault();
        }
    }

    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : Entity
    {
        protected readonly NpgsqlContext context;
        protected DbSet<TEntity> DbSet => context.Set<TEntity>();

        public Repository(NpgsqlContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<TEntity> ListAll()
        {
            return DbSet.AsEnumerable();
        }

        public TEntity? Find(params object[] ids)
        {
            return DbSet.Find(ids);
        }

        public TEntity Add(TEntity entity)
        {
            var entityEntry = DbSet.Add(entity);
            return entityEntry.Entity;
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public TEntity Update(TEntity entity)
        {
            var entityEntry = DbSet.Update(entity);
            return entityEntry.Entity;
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
        }

        public void UpdateRange(params TEntity[] entities)
        {
            DbSet.UpdateRange(entities);
        }

        public TEntity Remove(TEntity entity)
        {
            var entityEntry = DbSet.Remove(entity);
            return entityEntry.Entity;
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public void RemoveRange(params TEntity[] entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}