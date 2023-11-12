using Microsoft.EntityFrameworkCore;
using ProjectSchedulerAuth.Domain.Repositories;
using ProjectSchedulerAuth.Domain.Repositories.Base;
using ProjectSchedulerAuth.Infrastructure.DbTools;
using System.Linq.Expressions;

namespace ProjectSchedulerAuth.Infrastructure.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class, IEntityBase<TKey>
    {
        private readonly DbFactory _dbFactory;
        private DbSet<T> _dbSet;

        protected DbSet<T> DbSet
        {
            get => _dbSet ?? (_dbSet = _dbFactory.DbContext.Set<T>());
        }

        public Repository(DbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<T> List(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression);
        }

        public T GetByKey(TKey primaryKey)
        {
            return DbSet.SingleOrDefault(x => x.Id.Equals(primaryKey))!;
        }

        public T GetByKeyWithInclude(Func<T, bool> predicate,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = Include(includeProperties);
            return query.SingleOrDefault(predicate);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }

        private IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbSet.AsNoTracking();
            return includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
    }
}