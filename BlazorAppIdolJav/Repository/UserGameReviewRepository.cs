using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
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

        public async Task<bool> ChangeCommentHideStatusAsync(UserGameReviewData data)
        {
			try
			{
                var existing = await _context.Set<UserGameReview>().FirstOrDefaultAsync(c => c.Id == data.Id);
                if (existing == null)
                {
                    return false;
                }
                _context.Entry(existing).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
                return true;
            }
			catch
			{
				return false;
			}
        }

        public async Task<List<UserGameReview>> GetAllWithFilterAsync(IQueryable<UserGameReview> query, ReviewSearch search)
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

		public IQueryable<UserGameReview> GetQueryable()
		{
			return _context.UserGameReview.AsQueryable();
		}

		public async Task<StatisticReviewGameData> GetStatisticAboutGameAsync(string gameId)
		{
			try
			{
				var listReview = await _context.UserGameReview.Where(c => c.GameId == gameId).ToListAsync();
				int totalComment = listReview.Count;
				int totalPositive = listReview.Where(c => c.StarNumber >= 3).Count();
				int totalNegative = totalComment > 0 ? (totalComment - totalPositive) : 0 ;
				decimal positivePercent = totalComment > 0
										? (decimal)totalPositive / totalComment * 100
										: 0;
				decimal negativePercent = totalComment > 0
								   ? (decimal)totalNegative / totalComment * 100
								   : 0;

				decimal averageStar = totalComment > 0
					? listReview.Average(c => c.StarNumber)
					: 0;


				return new StatisticReviewGameData
				{
					Total = totalComment,
					PositivePercent = positivePercent,
					NegativePercent = negativePercent,
					AverageStarNumber = averageStar,
					QuantityPositive = totalPositive,
					QuantityNegative = totalNegative
				};
			}
			catch
			{
				return new StatisticReviewGameData();
			}
		}
	}
}
