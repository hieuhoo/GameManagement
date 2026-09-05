using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using GameManagement.Share.Extension;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class UserRepository : Repository<User>, IUserRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly StringExtension _extension;

		public UserRepository(ApplicationDbContext context, StringExtension extension) : base(context)
		{
			_context = context;
			_extension = extension;
		}

		public async Task<List<User>> GetAllWithFilterAsync(IQueryable<User> query, UserSearch search)
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

		public IQueryable<User> GetQueryable()
		{
			return _context.User.AsQueryable();
		}

		public async Task<bool> RegisterAccountAsync(User data)
		{
			try
			{
				data.PasswordHash = _extension.GetCharacterHash(data.PassWord); // hash thành chuỗi
				await AddAsync(data);
				return true;
			}
			catch (Exception ex)
			{ 
				return false;
			}
		}

		public async Task<bool> UpdateAccountAsync(User data)
		{
			try
			{
				var existing = await _context.Set<User>().FirstOrDefaultAsync(c => c.Id == data.Id);
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
			}
		}

		public async Task<bool> CheckExistUserInfoAsync(User data)
		{
			try
			{
				return await AnyAsync(x =>
					x.UserName == data.UserName ||
					x.Email == data.Email
				);
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<bool> CheckUserLoginAsync(User data)
		{
			try
			{
				return await AnyAsync(x =>
					x.UserName == data.UserName &&
					x.PassWord == data.PassWord
				);
			}
			catch
			{
				return false;
			}

		}

		public async Task<bool> CheckExistEmailAsync(string email)
		{
			try
			{
                return await AnyAsync(x =>
                    x.Email == email
                );
            }
			catch
			{
				return false;
			}
		}

		public async Task<User> GetUserInfoAsync(UserSearch search)
		{
			try
			{
				var filter = search.CreateFilter(GetQueryable());
                var data = await filter.FirstOrDefaultAsync();
				return data ?? new User();
			}
			catch (Exception ex)
			{
				return new User();
			}
		}
	}
}
