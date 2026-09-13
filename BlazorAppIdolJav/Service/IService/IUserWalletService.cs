using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface IUserWalletService
	{
		Task<RedeemTransactionResultData> SaveRedeemFromCodeAsync(string userId, string code,string type);
        Task<List<UserWalletData>> GetAllWithFilterAsync(UserWalletSearch search);
        Task<RedeemTransactionResultData> SaveMoneyFromTopupAsync(string userId, int money);
    }

    [DataContract]
    public class UserWalletSearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string UserId { get; set; }

        public IQueryable<UserWallet> CreateFilter(IQueryable<UserWallet> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (UserId.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.UserId == UserId);
            }
            return filter;
        }
    }
}
