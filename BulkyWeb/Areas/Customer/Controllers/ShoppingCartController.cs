using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Customer.Controllers;

public class ShoppingCartController : Controller
{
    [Area("Customer")]
    public IActionResult Index()
    {
        return View();
    }
}
