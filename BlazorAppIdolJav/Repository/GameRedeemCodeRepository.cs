using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
    public class GameRedeemCodeRepository : Repository<GameRedeemCode> , IGameRedeemCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public GameRedeemCodeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<GameRedeemCode>> GetAllWithFilterAsync(IQueryable<GameRedeemCode> query, GameRedeemSearch search)
        {
            try
            {
                var result = await query.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<GameRedeemCode> GetQueryable()
        {
            return _context.GameRedeemCode.AsQueryable();
        }

        public async Task<bool> SaveRedeemAsync(GameRedeemCode data)
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

        public async Task<bool> UpdateRedeemAsync(GameRedeemCode data)
        {
            try
            {
                var existing = await _context.Set<GameRedeemCode>().FirstOrDefaultAsync(c => c.Id == data.Id);
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

        public async Task<bool> DeleteRedeemAsync(GameRedeemCode data)
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

        public async Task<bool> SaveListRedeemAsync(List<GameRedeemCode> data)
        {
            try
            {
                await AddRangeAsync(data);
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
