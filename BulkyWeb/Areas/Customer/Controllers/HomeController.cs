
using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Utility;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController
    (ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    : Controller
{
    
    public IActionResult Index()
    {
        IEnumerable<Product> productList = unitOfWork.Product.GetAll(includeProperties: "Category");
        return View(productList);
    }
	public IActionResult Details(int productId)
	{
        ShoppingCart shoppingCart = new() {
            Product = unitOfWork.Product.Get(p => p.Id == productId, includeProperties: "Category"),
            Count = 1,
            ProductId = productId
        };
		
		return View(shoppingCart);
	}
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        shoppingCart.ApplicationUserId = userId;

        ShoppingCart cartFromDb = unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

        if (cartFromDb != null)
        {
            //shopping cart exists
            cartFromDb.Count += shoppingCart.Count;
            unitOfWork.ShoppingCart.Update(cartFromDb);
            unitOfWork.Save();
        }
        else
        {
            //add cart record
            unitOfWork.ShoppingCart.Add(shoppingCart);
            unitOfWork.Save();
            HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
        }
        TempData["success"] = "Cart updated successfully";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
