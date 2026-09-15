using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class UserGameLibraryRepository : Repository<UserGameLibrary>, IUserGameLibraryRepository
	{
		private readonly ApplicationDbContext _context;
		public UserGameLibraryRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<List<UserGameLibrary>> GetAllWithFilterAsync(IQueryable<UserGameLibrary> query, LibrarySearch search)
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

		public IQueryable<UserGameLibrary> GetQueryable()
		{
			return _context.UserGameLibrary.AsQueryable();
		}

		public async Task<bool> AddGameToLibraryAsync(UserGameLibrary data)
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
	}
}
