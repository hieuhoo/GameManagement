using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface IWalletTransactionHistoryService
	{
		Task<List<WalletTransactionHistoryData>> GetAllWithFilterAsync(TransactionHistorySearch search);
		Task<(int TotalSold, int? TotalRevenue)> GetTotalGameRevenue(string gameId);
	}

	[DataContract]
	public class TransactionHistorySearch
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public virtual string UserId { get; set; }
		[DataMember(Order = 3)]
		public virtual string RefId { get; set; }

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
			if (RefId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.ReferenceId == RefId);
			}
			return filter;
		}
	}
}
