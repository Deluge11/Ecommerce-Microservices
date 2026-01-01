using Authentication_Core.Interfaces;
using Authentication_Infrastructure.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Authentication_Core.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {

        public RepositoryBase(AppDbContext context)
        {
            _Entity = context.Set<TEntity>();
            _Context = context;
        }

        public AppDbContext _Context { get; }
        private DbSet<TEntity> _Entity { get; }


        public async Task<TEntity> Add(TEntity entity)
        {
            await _Entity.AddAsync(entity);
            return entity;
        }

        public async Task<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return await _Entity.SingleOrDefaultAsync(predicate);
        }
        public async Task<TEntity> GetById(int id)
        {
            return await _Entity.FindAsync(id);
        }
        public async Task<bool> Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return await _Entity.AnyAsync(predicate);
        }
    }
}
