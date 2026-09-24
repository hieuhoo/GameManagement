using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserGameReviewReplyRepository
	{
		Task<List<UserGameReviewReply>> GetAllWithFilterAsync(IQueryable<UserGameReviewReply> query,
									ReviewReplySearch search);
		IQueryable<UserGameReviewReply> GetQueryable();
		Task<bool> AddReplyForCommentAsync(UserGameReviewReply reply);
		Task<bool> UpdateReplyAsync(UserGameReviewReply reply);
	}

}
