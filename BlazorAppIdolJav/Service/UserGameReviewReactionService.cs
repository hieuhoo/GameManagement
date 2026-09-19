using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Service
{
	public class UserGameReviewReactionService : IUserGameReviewReactionService
	{
		private readonly ApplicationDbContext _context;
		private readonly IUserGameReviewReactionRepository _repo;
		private readonly IMapper _mapper;

		public UserGameReviewReactionService(ApplicationDbContext context, IUserGameReviewReactionRepository repo, IMapper mapper)
		{
			_context = context;
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<bool> ChangeReactionAsync(string reviewId, string userId, string type)
		{
			await using var transaction =
			   await _context.Database.BeginTransactionAsync();
			try
			{
				// 1. Check review
				var review = await _context.UserGameReview
					.FirstOrDefaultAsync(x => x.Id == reviewId);

				if (review == null)
				{
					return false;
				}

				// 2. Check user đã reaction chưa
				var existReac = await _context.UserGameReviewReaction
					.FirstOrDefaultAsync(x =>
						x.ReviewId == reviewId &&
						x.UserId == userId);

				// CASE 1: Chưa reaction thì add mới
				if (existReac == null)
				{
					var reaction = new UserGameReviewReaction
					{
						Id = ObjectExtentions.GenerateGuid(),
						ReviewId = reviewId,
						UserId = userId,
						ReactionType = type,
						CreateDate = DateTime.Now
					};

					await _context.UserGameReviewReaction
						.AddAsync(reaction);

					if (type == TypeReaction.Like.ToString())
					{
						review.LikeCount++;
					}
					else
					{
						review.HeartCount++;
					}
				}

				// CASE 2: Đã reaction
				else
				{
					// 2.1. Bấm lại đúng reaction hiện tại
					// => remove reaction
					if (existReac.ReactionType == type)
					{
						if (type == TypeReaction.Like.ToString())
						{
							review.LikeCount = Math.Max(0, review.LikeCount - 1);
						}
						else
						{
							review.HeartCount = Math.Max(0, review.HeartCount - 1);
						}

						_context.UserGameReviewReaction
							.Remove(existReac);
					}

					// 2.2. Đổi từ Like -> Heart
					//     hoặc Heart -> Like
					else
					{
						if (existReac.ReactionType == TypeReaction.Like.ToString())
						{
							review.LikeCount = Math.Max(0, review.LikeCount - 1);
							review.HeartCount++;
						}
						else
						{
							review.HeartCount = Math.Max(0, review.HeartCount - 1);
							review.LikeCount++;
						}

						existReac.ReactionType = type;
					}
				}
				review.UpdatedDate = DateTime.Now;
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

		public async Task<string> CheckDisplayMyReactionAsync(string userId, string reviewId)
		{
			try
			{
				var data = await _context.UserGameReviewReaction.FirstOrDefaultAsync(
									c => c.UserId == userId
									&& c.ReviewId == reviewId);
				if (data == null)
				{
					return string.Empty;
				}
				else
				{
					return data.ReactionType;
				}
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}
