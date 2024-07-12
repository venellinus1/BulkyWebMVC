using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("admin")]
[Authorize]
public class OrderController(IUnitOfWork unitOfWork)
	: Controller
{
	public IActionResult Index()
	{
		return View();
	}
    public IActionResult Details(int orderId)
    {
        OrderVM orderVM= new()
        {
            OrderHeader = unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
            OrderDetail = unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
        };
        return View(orderVM);
    }
    #region API CALLS
    [HttpGet]
	public IActionResult GetAll(string status)
	{
		IEnumerable<OrderHeader> orderHeaderList = unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        switch (status)
        {
            case "pending":
                orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
                break;
            case "inprocess":
                orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == StaticDetails.StatusInProcess);
                break;
            case "completed":
                orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == StaticDetails.StatusShipped);
                break;
            case "approved":
                orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == StaticDetails.StatusApproved);
                break;
            default:                
                break;
        }
        return Json(new { data = orderHeaderList });
	}

	#endregion
}
