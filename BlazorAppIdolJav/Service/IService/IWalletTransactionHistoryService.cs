using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface IWalletTransactionHistoryService
	{
        Task<List<WalletTransactionHistoryData>> GetAllWithFilterAsync(TransactionHistorySearch search);

	}

	[DataContract]
    public class TransactionHistorySearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string UserId { get; set; }

        public IQueryable<WalletTransactionHistory> CreateFilter(IQueryable<WalletTransactionHistory> filter)
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
