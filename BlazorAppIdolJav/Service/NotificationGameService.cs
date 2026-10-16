using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
	public class NotificationGameService : INotificationGameService
	{
		private readonly INotificationGameRepository _repo;
		readonly IMapper _mapper;

		public NotificationGameService(INotificationGameRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<bool> AddNotificationAsync(UserNotificationData data)
		{
			try
			{
				var noti = _mapper.Map<UserNotification>(data);
				var isSuccess = await _repo.AddNotificationAsync(noti);
				return isSuccess;
			}
			catch 
			{
				return false;
			}
		}

		public async Task<List<UserNotificationData>> GetAllWithFilterAsync(NotiSearch search)
		{
			var filter = search.CreateFilter(_repo.GetQueryable());
            var result = await _repo.GetAllWithFilterAsync(filter, search);
            var data = _mapper.Map<List<UserNotificationData>>(result);
            return data;
		}

		public async Task<bool> UpdateNotificationAsync(UserNotificationData data)
		{
			try
			{
				var noti = _mapper.Map<UserNotification>(data);
				var isSuccess = await _repo.UpdateNotificationAsync(noti);
				return isSuccess;
			}
			catch 
			{
				return false;
			}
		}
	}
}
