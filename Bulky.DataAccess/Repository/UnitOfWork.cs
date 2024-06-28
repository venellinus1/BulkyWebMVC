using Bulky.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Repository.IRepository;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;

namespace BulkyBook.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDBContext _db;
    public ICategoryRepository Category { get; private set; }

    public UnitOfWork(ApplicationDBContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
