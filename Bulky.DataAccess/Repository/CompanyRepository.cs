using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;


namespace Bulky.DataAccess.Repository;

public class CompanyRepository
    : Repository<Company>, ICompanyRepository
{
    private ApplicationDBContext _db;
    public CompanyRepository(ApplicationDBContext db) : base(db)
    {
        _db = db;
    }
    public void Save()
    {
        _db.SaveChanges();
    }

    public void Update(Company company)
    {
        _db.Companies.Update(company);
    }
}
