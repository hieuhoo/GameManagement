using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;

namespace GameManagement.Service
{
	public class WalletTransactionHistoryService : IWalletTransactionHistoryService
	{
		private readonly IWalletTransactionHistoryRepository _repo;
		readonly IMapper _mapper;

		public WalletTransactionHistoryService(IWalletTransactionHistoryRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<List<WalletTransactionHistoryData>> GetAllWithFilterAsync(TransactionHistorySearch search)
		{
			try
			{
				var filter = search.CreateFilter(_repo.GetQueryable());
				var result = await _repo.GetAllWithFilterAsync(filter, search);
				var data = _mapper.Map<List<WalletTransactionHistoryData>>(result);
				return data;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
