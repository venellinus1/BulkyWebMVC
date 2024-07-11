using Bulky.Models.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("admin")]
public class OrderController(IUnitOfWork unitOfWork)
	: Controller
{
	public IActionResult Index()
	{
		return View();
	}
	#region API CALLS
	[HttpGet]
	public IActionResult GetAll()
	{
		List<OrderHeader> orderHeaderList = unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
		return Json(new { data = orderHeaderList });
	}

	#endregion
}
