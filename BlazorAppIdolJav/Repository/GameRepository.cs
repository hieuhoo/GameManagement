using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class GameRepository : Repository<Game>, IGameRepository
	{
		private readonly ApplicationDbContext _context;

		public GameRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<List<Game>> GetAllWithFilterAsync(IQueryable<Game> query, GameSearch search)
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

		public IQueryable<Game> GetQueryable()
		{
			return _context.Game.AsQueryable();
		}

		public async Task<bool> SaveGameAsync(Game data)
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

		public async Task<bool> UpdateGameAsync(Game data)
		{
			try
			{
				var existing = await _context.Set<Game>().FirstOrDefaultAsync(c => c.Id == data.Id);
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

		public async Task<bool> DeleteGameAsync(Game data)
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
