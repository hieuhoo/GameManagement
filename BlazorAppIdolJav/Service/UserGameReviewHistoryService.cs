using AutoMapper;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;

namespace GameManagement.Service
{
	public class UserGameReviewHistoryService : IUserGameReviewHistoryService
	{
		private readonly IUserGameReviewHistoryRepository _repo;
		private readonly IMapper _mapper;
		public UserGameReviewHistoryService(IUserGameReviewHistoryRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<List<UserGameReviewHistoryData>> GetAllWithFilterAsync(ReviewHistorySearch search)
		{
			try
			{
				var filter = search.CreateFilter(_repo.GetQueryable());
				var result = await _repo.GetAllWithFilterAsync(filter, search);
				var data = _mapper.Map<List<UserGameReviewHistoryData>>(result);
				return data;
			}
			catch
			{
				return new List<UserGameReviewHistoryData>();
			}
		}
	}
}
