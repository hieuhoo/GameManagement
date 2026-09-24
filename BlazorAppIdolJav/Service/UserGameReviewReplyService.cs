using AutoMapper;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;

namespace GameManagement.Service
{
	public class UserGameReviewReplyService : IUserGameReviewReplyService
	{
		private readonly ApplicationDbContext _context;
		private readonly IUserGameReviewReplyRepository _repo;
		private readonly IMapper _mapper;

		public UserGameReviewReplyService(ApplicationDbContext context, IUserGameReviewReplyRepository repo, IMapper mapper)
		{
			_context = context;
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<bool> AddReplyForCommentAsync(UserGameReviewReplyData data)
		{
			try
			{
				var reply = _mapper.Map<UserGameReviewReply>(data);
				var isSuccess = await _repo.AddReplyForCommentAsync(reply);
				return isSuccess;
			}
			catch
			{
				return false;
			}
		}

		public async Task<List<UserGameReviewReplyData>> GetAllWithFilterAsync(ReviewReplySearch search)
		{
			try
			{
				var filter = search.CreateFilter(_repo.GetQueryable());
				var result = await _repo.GetAllWithFilterAsync(filter, search);
				var data = _mapper.Map<List<UserGameReviewReplyData>>(result);
				return data;
			}
			catch
			{
				return new List<UserGameReviewReplyData>();
			}
		}

		public async Task<bool> UpdateReplyAsync(UserGameReviewReplyData data)
		{
			try
			{
				var reply = _mapper.Map<UserGameReviewReply>(data);
				var isSuccess = await _repo.UpdateReplyAsync(reply);
				return isSuccess;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
