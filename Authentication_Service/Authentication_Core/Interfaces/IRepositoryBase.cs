



using System.Linq.Expressions;

namespace Authentication_Core.Interfaces
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task<TEntity> GetById(int id);
        Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<bool> Exists(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> Add(TEntity entity);
    }
}
