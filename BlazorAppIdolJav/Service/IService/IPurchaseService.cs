using GameManagement.Share.ClassData;

namespace GameManagement.Service.IService
{
	public interface IPurchaseService
	{
		Task<PurchaseGameResultData> ProcessPurchaseTransactionAsync(PaymentGameInforData data);
	}
}
