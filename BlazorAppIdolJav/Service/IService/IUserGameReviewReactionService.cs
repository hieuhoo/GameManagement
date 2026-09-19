namespace GameManagement.Service.IService
{
	public interface IUserGameReviewReactionService
	{
		Task<bool> ChangeReactionAsync(string reviewId, string userId, string type);
		Task<string> CheckDisplayMyReactionAsync(string userId, string reviewId);
	}
}
