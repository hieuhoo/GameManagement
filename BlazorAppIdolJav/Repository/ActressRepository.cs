using BlazorAppIdolJav.Data;
using BlazorAppIdolJav.Repository.IRepository;
using BlazorAppIdolJav.Service.IService;
using BlazorAppIdolJav.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppIdolJav.Repository
{
    public class ActressRepository : IActressRepository
    {
        private readonly ApplicationDbContext _context;

        public ActressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Actress>> GetAllWithFilterAsync(IQueryable<Actress> query , ActressSearch search)
        {
            try
            {
                var result = await query.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Actress> GetQueryable()
        {
            return _context.Actress.AsQueryable();
        }
    }
}
