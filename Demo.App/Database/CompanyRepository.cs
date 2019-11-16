using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.App.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Pipaslot.Authorization;

namespace Demo.App.Database
{
    public class CompanyRepository : IResourceInstanceProvider
    {
        private readonly DatabaseFactory _databaseFactory;

        public CompanyRepository(DatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }
        public async Task<ICollection<ResourceInstance>> GetAllInstancesAsync()
        {
            using (var db = _databaseFactory.Create())
            {
                return await db.Company
                    .Select(c => new ResourceInstance
                    {
                        Id = c.Id.ToString(),
                        Name = c.Name
                    })
                    .ToListAsync();
            }
        }

        public async Task<ICollection<Company>> GetAll()
        {
            using (var db = _databaseFactory.Create())
            {
                return await db.Company.ToListAsync();
            }
        }

        public async Task<Company> Get(int id)
        {
            using (var db = _databaseFactory.Create())
            {
                return await db.Company.FirstOrDefaultAsync(c => c.Id == id);
            }
        }
    }
}
