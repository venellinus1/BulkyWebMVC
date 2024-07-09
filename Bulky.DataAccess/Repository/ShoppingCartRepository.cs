using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Models;


namespace Bulky.DataAccess.Repository;

public class ShoppingCartRepository
    : Repository<ShoppingCart>, IShoppingCartRepository
{
    private ApplicationDBContext _db;
    public ShoppingCartRepository(ApplicationDBContext db) : base(db)
    {
        _db = db;
    }
    public void Save()
    {
        _db.SaveChanges();
    }

    public void Update(ShoppingCart shoppingCart)
    {
        _db.Update(shoppingCart);
    }
}
