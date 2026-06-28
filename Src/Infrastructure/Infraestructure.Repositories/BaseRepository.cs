using Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetById(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual Task Update(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}