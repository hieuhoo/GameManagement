using GameManagement.Share.ClassData;

namespace GameManagement.Service.IService
{
	public interface IUserGameReviewReactionService
	{
		Task<bool> ChangeReactionAsync(string reviewId, string userId, string type);
		Task<string> CheckDisplayMyReactionAsync(string userId, string reviewId);
		Task<List<ReactionPersonInfoData>> GetListUsersReactAsync(string reviewId);
	}
}
