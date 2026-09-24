using AutoMapper;
using GameManagement.CoreConfig.Extensions;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
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

					// +1 reaction mới
					ChangeReactionCount(review, type, 1);
				}

				// CASE 2: Đã reaction
				else
				{
					// Bấm lại reaction hiện tại
					if (existReac.ReactionType == type)
					{
						// -1 reaction cũ
						ChangeReactionCount(review, type, -1);

						_context.UserGameReviewReaction.Remove(existReac);
					}
					else
					{
						// -1 reaction cũ
						ChangeReactionCount(
							review,
							existReac.ReactionType,
							-1);

						// +1 reaction mới
						ChangeReactionCount(
							review,
							type,
							1);

						// Update loại reaction
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

		public async Task<string> CheckDisplayMyReactionAsync(string userId, string reviewId, string replyId = null)
		{
			try
			{
				UserGameReviewReaction dataReact;
				if (replyId.IsNullOrEmpty())
				{
					// Reaction của review gốc
					dataReact = await _context.UserGameReviewReaction
						.FirstOrDefaultAsync(c =>
							c.UserId == userId &&
							c.ReviewId == reviewId &&
							c.ReplyId == null);
				}
				else
				{
					// Reaction của reply
					dataReact = await _context.UserGameReviewReaction
						.FirstOrDefaultAsync(c =>
							c.UserId == userId &&
							c.ReviewId == reviewId &&
							c.ReplyId == replyId);
				}
				return dataReact?.ReactionType ?? string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}

		public async Task<List<ReactionPersonInfoData>> GetListUsersReactAsync(string reviewId)
		{
			try
			{
				var result = await _repo.GetListUsersReactAsync(reviewId);
				return result;
			}
			catch
			{
				return new List<ReactionPersonInfoData>();
			}
		}

		void ChangeReactionCount(
				UserGameReview review,
				string reactionType,
				int amount)
		{
			if (reactionType == TypeReaction.Like.ToString())
			{
				review.LikeCount = Math.Max(0, review.LikeCount + amount);
			}
			else if (reactionType == TypeReaction.Heart.ToString())
			{
				review.HeartCount = Math.Max(0, review.HeartCount + amount);
			}
			else if (reactionType == TypeReaction.Funny.ToString())
			{
				review.FunnyCount = Math.Max(0, review.FunnyCount + amount);
			}
		}

		public async Task<List<ReactionPersonInfoData>> GetAllWithFilterAsync(PersonReactionSearch search)
		{
			try
			{
				var filter = search.CreateFilter(_repo.GetQueryable());
				var result = await _repo.GetAllWithFilterAsync(filter, search);
				var reactionMap = _mapper.Map<List<UserGameReviewReactionData>>(result);
				var finalData = _mapper.Map<List<ReactionPersonInfoData>>(reactionMap);
				return finalData;
			}
			catch
			{
				return new List<ReactionPersonInfoData>();
			}
		}


		public async Task<bool> ChangeReactionForReplyAsync(
				string reviewId,
				string replyId,
				string userId)
		{
			await using var transaction =
				await _context.Database.BeginTransactionAsync();

			try
			{
				// 1. Check reply
				var reply = await _context.UserGameReviewReply
					.FirstOrDefaultAsync(x =>
						x.Id == replyId &&
						x.ReviewId == reviewId);

				if (reply == null)
				{
					return false;
				}

				// 2. Check user đã like reply này chưa
				var existReac = await _context.UserGameReviewReaction
					.FirstOrDefaultAsync(x =>
						x.ReviewId == reviewId &&
						x.ReplyId == replyId &&
						x.UserId == userId);

				// CASE 1: Chưa like -> thêm like
				if (existReac == null)
				{
					var reaction = new UserGameReviewReaction
					{
						Id = ObjectExtentions.GenerateGuid(),
						ReviewId = reviewId,
						ReplyId = replyId,
						UserId = userId,
						ReactionType = TypeReaction.Like.ToString(),
						CreateDate = DateTime.Now
					};

					await _context.UserGameReviewReaction
						.AddAsync(reaction);

					reply.LikeCount++;
				}
				// CASE 2: Đã like -> bỏ like
				else
				{
					_context.UserGameReviewReaction
						.Remove(existReac);

					reply.LikeCount = Math.Max(
						0,
						reply.LikeCount - 1);
				}

				reply.UpdatedDate = DateTime.Now;

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
