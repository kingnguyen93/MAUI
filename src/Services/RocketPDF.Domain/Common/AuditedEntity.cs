using RocketPDF.AutoMapper;

namespace RocketPDF.Domain.Entities
{
    [Serializable]
    public abstract class AuditedEntity : Entity
    {
        [IgnoreMap]
        public virtual bool IsDeleted { get; set; }

        [IgnoreMap]
        public virtual DateTime CreatedDate { get; set; }

        [IgnoreMap]
        public virtual Guid CreatedUser { get; set; }

        [IgnoreMap]
        public virtual DateTime? UpdatedDate { get; set; }

        [IgnoreMap]
        public virtual Guid? UpdatedUser { get; set; }

        public void SetCreatedTime(Guid? userId = default)
        {
            CreatedDate = DateTime.UtcNow;
            CreatedUser = userId ?? Guid.Empty;
        }

        public void SetUpdatedTime(Guid? userId = default)
        {
            UpdatedDate = DateTime.UtcNow;
            UpdatedUser = userId ?? Guid.Empty;
        }
    }

    [Serializable]
    public abstract class AuditedEntity<TKey> : AuditedEntity, IEntity<TKey>
        where TKey : IComparable<TKey>
    {
        public virtual required TKey Id { get; set; }
    }
}