using RocketPDF.Domain.Entities;

namespace RocketPDF.Infrastructure.Repositories
{
    public interface IRepository<TEntity, TKey> : IRepository<TEntity>
        where TEntity : IEntity<TKey>
        where TKey : IComparable<TKey>
    {
        TEntity? Get(TKey id);
    }

    public interface IRepository<TEntity>
        where TEntity : IEntity
    {
        IEnumerable<TEntity> ListAll();

        TEntity? Find(params object[] ids);

        TEntity Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        void UpdateRange(params TEntity[] entities);

        TEntity Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);

        void RemoveRange(params TEntity[] entities);
    }
}