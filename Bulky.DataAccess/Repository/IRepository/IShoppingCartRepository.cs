using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bulky.DataAccess.Repository.IRepository;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    void Update(ShoppingCart shoppingCart);
}
