namespace RocketPDF.Infrastructure.Models
{
    public abstract class AuditedEntityDto<TKey> : EntityDto<TKey>
        where TKey : IComparable<TKey>
    {
        public virtual DateTime CreatedDate { get; set; }

        public virtual Guid CreatedUser { get; set; }

        public virtual DateTime? UpdatedDate { get; set; }

        public virtual Guid? UpdatedUser { get; set; }
    }
}