
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
		Product product = unitOfWork.Product.Get(p => p.Id == productId, includeProperties: "Category");
		return View(product);
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
