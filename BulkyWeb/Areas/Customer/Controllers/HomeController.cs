
using Bulky.Models;
using Bulky.Models.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
