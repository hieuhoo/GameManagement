using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Share.ClassDB;
using GameManagement.WebInterface.User;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
	public class UserGameReviewRepository : Repository<UserGameReview>, IUserGameReviewRepository
	{
		private readonly ApplicationDbContext _context;
		public UserGameReviewRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}
	}
}
