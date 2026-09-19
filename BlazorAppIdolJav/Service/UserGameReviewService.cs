using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using GameManagement.WebInterface.User;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Service
{
	public class UserGameReviewService : IUserGameReviewService
	{
		private readonly IUserGameReviewRepository _repo;
		readonly IMapper _mapper;
		private readonly ApplicationDbContext _context;


		public UserGameReviewService(IUserGameReviewRepository repo, IMapper mapper, ApplicationDbContext context)
		{
			_repo = repo;
			_mapper = mapper;
			_context = context;
		}

		public async Task<bool> CreateReviewAsync(UserGameReviewData data)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				var review = new UserGameReview
				{
					Id = ObjectExtentions.GenerateGuid(),
					GameId = data.GameId,
					UserId = data.UserId,
					StarNumber = data.StarNumber,
					IsRecommend = data.IsRecommend,
					Comment = data.Comment,
					CreateDate = DateTime.Now,
					UpdatedDate = DateTime.Now,
					IsHidden = false,
					LikeCount = 0,
					HeartCount = 0,
				};

				await _context.UserGameReview.AddAsync(review);

				var history = new UserGameReviewHistory
				{
					Id = ObjectExtentions.GenerateGuid(),
					ReviewId = review.Id,
					UserId = data.UserId,
					GameId = data.GameId,
					PrevComment = null,
					CurrentComment = review.Comment,
					PrevStarNumber = null,
					CurrentStarNumber = review.StarNumber,
					CreateDate = DateTime.Now,
					PrevIsRecommend = null,
					CurrentIsRecommend = review.IsRecommend,
				};

				await _context.UserGameReviewHistory.AddAsync(history);

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				return true;
			}
			catch
			{
				await transaction.RollbackAsync();
				return false;
			}
		}

		public async Task<List<UserGameReviewData>> GetAllWithFilterAsync(ReviewSearch search)
		{
			try
			{
				var filter = search.CreateFilter(_repo.GetQueryable());
				var result = await _repo.GetAllWithFilterAsync(filter, search);
				var data = _mapper.Map<List<UserGameReviewData>>(result);
				return data;
			}
			catch
			{
				return new List<UserGameReviewData>();
			}
		}

		public  async Task<StatisticReviewGameData> GetStatisticAboutGameAsync(string gameId)
		{
			try
			{
				var result = await _repo.GetStatisticAboutGameAsync(gameId);
				return result;
			}
			catch
			{
				return new StatisticReviewGameData();
			}
		}

		public async Task<bool> UpdateReviewAsync(UserGameReviewData data)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				var review = await _context.UserGameReview
					.FirstOrDefaultAsync(x => x.Id == data.Id);

				if (review == null)
				{
					await transaction.RollbackAsync();
					return false;
				}

				var now = DateTime.Now;

				// Lưu dữ liệu cũ để tạo history
				var prevComment = review.Comment;
				var prevStarNumber = review.StarNumber;
				var prevIsRecommend = review.IsRecommend;

				// Update review hiện tại
				review.StarNumber = data.StarNumber;
				review.IsRecommend = data.IsRecommend;
				review.Comment = data.Comment;
				review.UpdatedDate = now;
				review.IsAnonymous = data.IsAnonymous;

				// Tạo history
				var history = new UserGameReviewHistory
				{
					Id = ObjectExtentions.GenerateGuid(),
					ReviewId = review.Id,
					UserId = review.UserId,
					GameId = review.GameId,

					PrevComment = prevComment,
					CurrentComment = review.Comment,

					PrevStarNumber = prevStarNumber,
					CurrentStarNumber = review.StarNumber,

					PrevIsRecommend = prevIsRecommend,
					CurrentIsRecommend = review.IsRecommend,

					CreateDate = now
				};

				await _context.UserGameReviewHistory.AddAsync(history);

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return true;
			}
			catch
			{
				await transaction.RollbackAsync();
				return false;
			}
		}
	}
}
