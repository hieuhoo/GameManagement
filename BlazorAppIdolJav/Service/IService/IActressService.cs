using BlazorAppIdolJav.CoreConfig.Extensions;
using BlazorAppIdolJav.Share.ClassData;
using BlazorAppIdolJav.Share.ClassDB;
using System.Runtime.Serialization;

namespace BlazorAppIdolJav.Service.IService
{
    public interface IActressService
    {
        Task<List<ActressData>> GetAllWithFilterAsync(ActressSearch search);
    }

    [DataContract]
    public class ActressSearch
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public virtual string CompanyId { get; set; }
        [DataMember(Order = 3)]
        public virtual string Country { get; set; }
        [DataMember(Order = 4)]
        public virtual string Name { get; set; }
        [DataMember(Order = 5)]
        public virtual int Age { get; set; }

        public IQueryable<Actress> CreateFilter(IQueryable<Actress> filter)
        {
            if (Id.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Id == Id);
            }
            if (CompanyId.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.CompanyId == CompanyId);
            }
            if (Country.IsNotNullOrEmpty())
            {
                filter = filter.Where(x => x.Country == Country);
            }
            if (Age != 0)
            {
                filter = filter.Where(x => x.Age == Age);
            }
            return filter;
        }
    }
}
