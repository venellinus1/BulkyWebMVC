using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;


namespace Bulky.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private ApplicationDBContext _db;
    public ProductRepository(ApplicationDBContext db) : base(db)
    {
        _db = db;
    }
    public void Update(Product product)
    {
        var objFromDb = _db.Products.FirstOrDefault(u => u.Id == product.Id);
        if (objFromDb != null)
        {
            objFromDb.Title = product.Title;
            objFromDb.ISBN = product.ISBN;
            objFromDb.Price = product.Price;
            objFromDb.Price50 = product.Price50;
            objFromDb.ListPrice = product.ListPrice;
            objFromDb.Price100 = product.Price100;
            objFromDb.Description = product.Description;
            objFromDb.Author = product.Author;
            if (objFromDb.ImageUrl != null)
            {
                objFromDb.ImageUrl = objFromDb.ImageUrl;
            }
        }
    }
}
