namespace ProjectSchedulerAuth.Domain.Repositories.Base
{
    public interface IEntityBase<TKey>
    {
        TKey Id { get; }
    }
}