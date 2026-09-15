using GameManagement.Share.ClassData;

namespace GameManagement.Service.IService
{
	public interface IPurchaseService
	{
		Task<PurchaseGameResultData> ProcessPurchaseTransactionAsync(string userId, string gameId, decimal price);
	}
}
