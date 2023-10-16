namespace RocketPDF.Infrastructure.Models
{
    public abstract class EntityDto<TKey>
        where TKey : IComparable<TKey>
    {
        public virtual required TKey Id { get; set; }
    }
}