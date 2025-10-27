using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static GameManagement.CoreConfig.Repository.InterfaceRepository;

namespace GameManagement.CoreConfig.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(string id) => await _dbSet.FindAsync(id);
        public async Task<T?> GetByIdNoTrackingAsync(string id)
             => await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);

        public async Task AddAsync(T entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(T entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }

}
