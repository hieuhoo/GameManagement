using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
    public class GameTypeRepository : Repository<GameType> , IGameTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public GameTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<GameType>> GetAllWithFilterAsync(IQueryable<GameType> query, GameTypeSearch search)
        {
            try
            {
                var result = await query.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return new List<GameType>();
                throw;
            }
        }

        public IQueryable<GameType> GetQueryable()
        {
            return _context.GameType.AsQueryable();
        }

        public async Task<bool> SaveGameTypeAsync(GameType data)
        {
            try
            {
                await AddAsync(data);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> UpdateGameTypeAsync(GameType data)
        {
            try
            {
                var existing = await _context.Set<GameType>().FirstOrDefaultAsync(c => c.Id == data.Id);
                if (existing == null)
                {
                    return false;
                }
                _context.Entry(existing).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> DeleteGameTypeAsync(GameType data)
        {
            try
            {
                await DeleteAsync(data.Id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
    }
}
