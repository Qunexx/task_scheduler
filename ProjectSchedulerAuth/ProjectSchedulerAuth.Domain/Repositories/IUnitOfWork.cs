namespace ProjectSchedulerAuth.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public Task<int> CommitAsync();
    }
}