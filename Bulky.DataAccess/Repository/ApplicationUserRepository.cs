using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;


namespace Bulky.DataAccess.Repository
{
    public class ApplicationUserRepository
        : Repository<ApplicationUserRepository>, IApplicationUserRepository
    {
        private ApplicationDBContext _db;
        public ApplicationUserRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
    }
}
