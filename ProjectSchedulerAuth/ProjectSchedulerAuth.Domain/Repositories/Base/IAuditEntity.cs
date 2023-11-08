namespace ProjectSchedulerAuth.Domain.Repositories.Base
{
    public interface IAuditEntity
    {
        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        string UpdatedBy { get; set; }
    }

    public interface IAuditEntity<TKey> : IAuditEntity, IDeleteEntity<TKey>
    {
    }
}