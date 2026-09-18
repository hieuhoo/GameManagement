using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using GameManagement.WebInterface.User;

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
					UpdatedDate = DateTime.Now
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
	}
}
