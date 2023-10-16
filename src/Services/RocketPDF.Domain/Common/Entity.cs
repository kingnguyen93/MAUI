namespace RocketPDF.Domain.Entities
{
    public interface IEntity
    {
    }

    [Serializable]
    public abstract class Entity : IEntity
    {
    }

    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; set; }
    }

    [Serializable]
    public abstract class Entity<TKey> : Entity, IEntity<TKey>
        where TKey : IComparable<TKey>
    {
        public virtual required TKey Id { get; set; }
    }
}