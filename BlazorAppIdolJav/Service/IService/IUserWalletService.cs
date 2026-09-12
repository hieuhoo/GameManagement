using GameManagement.Share.ClassData;

namespace GameManagement.Service.IService
{
	public interface IUserWalletService
	{
		Task<RedeemTransactionResultData> SaveRedeemFromCodeAsync(string userId, string code,string type);
	}
}
