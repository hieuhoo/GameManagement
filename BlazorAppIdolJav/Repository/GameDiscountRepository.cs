using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class GameDiscountRepository : Repository<GameDiscount>, IGameDiscountRepository

	{
		private readonly ApplicationDbContext _context;

		public GameDiscountRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<List<GameDiscount>> GetAllWithFilterAsync(IQueryable<GameDiscount> query, DiscountSearch search)
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

		public IQueryable<GameDiscount> GetQueryable()
		{
			return _context.GameDiscount.AsQueryable();
		}

		public async Task<bool> SaveDiscountAsync(GameDiscount data)
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

		public async Task<bool> UpdateDiscountAsync(GameDiscount data)
		{
			try
			{
				var existing = await _context.Set<GameDiscount>().FirstOrDefaultAsync(c => c.Id == data.Id);
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

		public async Task<bool> DeleteDiscountAsync(GameDiscount data)
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

		public async Task<bool> CheckDiscountOverlapTimeAsync(string id, DateTime? start, DateTime? end)
		{
			try
			{
				return await AnyAsync(x =>
					x.GameId == id &&
					x.StartDate < end &&
					x.EndDate > start
				);
			}
			catch
			{
				return false;
			}
		}
	}
}
