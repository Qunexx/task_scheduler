namespace ProjectSchedulerAuth.Domain.Repositories.Base
{
    public interface IDeleteEntity
    {
        bool IsDeleted { get; set; }
    }

    public interface IDeleteEntity<TKey> : IDeleteEntity, IEntityBase<TKey>
    {
    }
}