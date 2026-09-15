using GameManagement.CoreConfig.Extensions;
using GameManagement.Share.ClassData;
using GameManagement.Share.ClassDB;
using System.Runtime.Serialization;

namespace GameManagement.Service.IService
{
	public interface IUserGameLibraryService
	{
		Task<List<UserGameLibraryData>> GetAllWithFilterAsync(LibrarySearch search);
		Task<bool> AddGameToLibraryAsync(UserGameLibraryData data);
		
	}

	[DataContract]
	public class LibrarySearch
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public virtual string UserId { get; set; }
		[DataMember(Order = 3)]
		public virtual string GameId { get; set; }

		public IQueryable<UserGameLibrary> CreateFilter(IQueryable<UserGameLibrary> filter)
		{
			if (Id.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.Id == Id);
			}
			if (UserId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.UserId == UserId);
			}
			if (GameId.IsNotNullOrEmpty())
			{
				filter = filter.Where(x => x.GameId == GameId);
			}
			return filter;
		}
	}
}
