using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;
using static GameManagement.Share.Extension.EnumExtension;

namespace GameManagement.Repository
{
    public class UserGameReviewReactionRepository : Repository<UserGameReviewReaction>, IUserGameReviewReactionRepository
    {
        private readonly ApplicationDbContext _context;

        public UserGameReviewReactionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<UserGameReviewReaction> GetQueryable()
        {
            return _context.UserGameReviewReaction.AsQueryable();
        }

        public async Task<List<ReactionPersonInfoData>> GetListUsersReactAsync(string reviewId)
        {
            try
            {
                var reviewData = await _context.UserGameReview.Where(c => c.Id == reviewId)
                                                                .FirstOrDefaultAsync() ?? new UserGameReview();
                var listUser = await _context.User.Where(c => c.Role == UserRole.Normal.ToString()).ToListAsync();
                var userDict = listUser.ToDictionary(c => c.Id, c => c.UserName);
                var data = await _context.UserGameReviewReaction.Where(c => c.ReviewId == reviewId)
                                                                .Select(c => new ReactionPersonInfoData
                                                                {
                                                                    UserId = c.UserId,
                                                                    IsAnonymous = false,
                                                                    ReactionType = c.ReactionType
                                                                })
                                                                .ToListAsync();
                foreach (var item in data)
                {
                    if (item.UserId != null && userDict.TryGetValue(item.UserId, out var name))
                    {
                        item.UserName = name;
                    }
                }
                return data;
            }
            catch
            {
                return new List<ReactionPersonInfoData>();
            }
        }
    }
}
