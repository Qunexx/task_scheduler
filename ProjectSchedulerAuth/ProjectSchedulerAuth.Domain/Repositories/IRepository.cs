using System.Linq.Expressions;

namespace ProjectSchedulerAuth.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> List(Expression<Func<T, bool>> expression);
    }
}