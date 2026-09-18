using GameManagement.Share.ClassData;

namespace GameManagement.Service.IService
{
	public interface IUserGameReviewService
	{
		Task<bool> CreateReviewAsync(UserGameReviewData data);
	}
}
