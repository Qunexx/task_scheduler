using ProjectSchedulerAuth.Domain.Repositories.Base;

namespace ProjectSchedulerAuth.Infrastructure.Repositories.Base
{
    public abstract class DeleteEntity<TKey> : EntityBase<TKey>, IDeleteEntity<TKey>
    {
        public bool IsDeleted { get; set; }
    }
}