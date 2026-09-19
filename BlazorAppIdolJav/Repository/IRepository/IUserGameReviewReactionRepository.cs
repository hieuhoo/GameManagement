using GameManagement.Share.ClassDB;

namespace GameManagement.Repository.IRepository
{
	public interface IUserGameReviewReactionRepository
	{
		IQueryable<UserGameReviewReaction> GetQueryable();

	}
}
