using Authentication_Core.Interfaces;
using Authentication_Infrastructure.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;




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


        public async Task Add(TEntity entity)
        {
            await _Entity.AddAsync(entity);
        }
        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _Entity.ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _Entity.FindAsync(id);
        } 
    }
}
