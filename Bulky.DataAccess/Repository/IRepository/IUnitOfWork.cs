using Bulky.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    void Save();
}
