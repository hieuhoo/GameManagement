using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserGameReviewReactionRepository
	{
		IQueryable<UserGameReviewReaction> GetQueryable();
		Task<List<ReactionPersonInfoData>> GetListUsersReactAsync(string reviewId);

    }
}
