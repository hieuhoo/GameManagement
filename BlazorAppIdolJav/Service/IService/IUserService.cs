using BlazorAppIdolJav.CoreConfig.Extensions;
using BlazorAppIdolJav.Share.ClassData;
using BlazorAppIdolJav.Share.ClassDB;
using System.Runtime.Serialization;

namespace BlazorAppIdolJav.Service.IService
{
    public interface IUserService
    {
        Task<List<UserData>> GetAllWithFilterAsync(UserSearch search);
        Task<bool> RegisterAccountAsync(UserData data);
        Task<bool> CheckExistUserInfoAsync(UserData data);

    }

    [DataContract]
    public class UserSearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string UserName { get; set; }
        [DataMember(Order = 3)]
        public virtual string PassWord { get; set; }
        [DataMember(Order = 4)]
        public virtual string Name { get; set; }
        [DataMember(Order = 5)]
        public virtual string Email { get; set; }
        [DataMember(Order = 6)]
        public virtual string Role { get; set; }
        [DataMember(Order = 7)]
        public virtual int PhoneNumber { get; set; }

        public IQueryable<User> CreateFilter(IQueryable<User> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (UserName.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.UserName == UserName);
            }
            if (PassWord.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.PassWord == PassWord);
            }
            if (Name.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Name == Name);
            }
            return filter;
        }
    }
}
