using AutoMapper;
using GameManagement.CoreConfig.Repository;
using GameManagement.Data;
using GameManagement.Repository.IRepository;
using GameManagement.Service.IService;
using GameManagement.Share.ClassDB;
using Microsoft.EntityFrameworkCore;

namespace GameManagement.Repository
{
    public class GameCompanyRepository : Repository<GameCompany> , IGameCompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public GameCompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<GameCompany>> GetAllWithFilterAsync(IQueryable<GameCompany> query, GameCompanySearch search)
        {
            try
            {
                var result = await query.ToListAsync();
                return result;
            }
            catch(Exception ex) 
            {
                return new List<GameCompany>();
                throw;
            }
        }

        public IQueryable<GameCompany> GetQueryable()
        {
            return _context.GameCompany.AsQueryable();
        }

        public async Task<bool> SaveCompanyAsync(GameCompany data)
        {
            try
            {
                await AddAsync(data);
                return true;
            }
            catch( Exception ex ) 
            {
                return false;
                throw ;
            }
        }

        public async Task<bool> UpdateCompanyAsync(GameCompany data)
        {
            try
            {
                var existing = await _context.Set<GameCompany>().FirstOrDefaultAsync(c => c.Id == data.Id);
                if (existing == null)
                {
                    return false;
                }    
                _context.Entry(existing).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> DeleteCompanyAsync(GameCompany data)
        {
            try
            {
                await DeleteAsync(data.Id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
    }
}
