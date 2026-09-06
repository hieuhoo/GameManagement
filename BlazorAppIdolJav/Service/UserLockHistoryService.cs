using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
	public class UserLockHistoryService : IUserLockHistoryService
	{
		private readonly IUserLockHistoryRepository _repo;
		readonly IMapper _mapper;

		public UserLockHistoryService(IUserLockHistoryRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<bool> AddLockHistoryAsync(UserLockHistoryData data)
		{
			try
			{
				var history = _mapper.Map<UserLockHistory>(data);
                var isSuccess = await _repo.AddLockHistoryAsync(history);
                return isSuccess;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
