using ProjectSchedulerAuth.Domain.Repositories.Base;
using System.Linq.Expressions;

namespace ProjectSchedulerAuth.Domain.Repositories
{
    public interface IRepository<T, TKey> where T : class, IEntityBase<TKey>
    {
        public void Add(T entity);
        public void Update(T entity);
        public T GetByKey(TKey primaryKey);
        public T GetByKeyWithInclude(Func<T, bool> predicate,
            params Expression<Func<T, object>>[] includeProperties);
        public void Delete(T entity);
        public IQueryable<T> List(Expression<Func<T, bool>> expression);
    }
}