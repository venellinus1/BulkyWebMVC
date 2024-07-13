using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("admin")]
[Authorize]
public class OrderController(IUnitOfWork unitOfWork)
	: Controller
{
    [BindProperty]
    public OrderVM OrderVM { get; set; }
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
    [HttpPost]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public IActionResult UpdateOrderDetail()
    {
        var orderHeaderFromDb = unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
        orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = OrderVM.OrderHeader.City;
        orderHeaderFromDb.State = OrderVM.OrderHeader.State;
        orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
        {
            orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
        }
        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
        {
            orderHeaderFromDb.Carrier = OrderVM.OrderHeader.TrackingNumber;
        }
        unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        unitOfWork.Save();

        TempData["Success"] = "Order Details Updated Successfully.";


        return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
    }

    [HttpPost]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public IActionResult StartProcessing()
    {
        unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, StaticDetails.StatusInProcess);
        unitOfWork.Save();
        TempData["Success"] = "Order Details Updated Successfully.";
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public IActionResult ShipOrder()
    {

        var orderHeader = unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
        orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
        orderHeader.OrderStatus = StaticDetails.StatusShipped;
        orderHeader.ShippingDate = DateTime.Now;
        if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
        {
            orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
        }

        unitOfWork.OrderHeader.Update(orderHeader);
        unitOfWork.Save();
        TempData["Success"] = "Order Shipped Successfully.";
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public IActionResult CancelOrder()
    {

        var orderHeader = unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

        if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
        }
        else
        {
            unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
        }
        unitOfWork.Save();
        TempData["Success"] = "Order Cancelled Successfully.";
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

    }

    #region API CALLS
    [HttpGet]
	public IActionResult GetAll(string status)
	{
		IEnumerable<OrderHeader> orderHeaderList = unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
        {
            orderHeaderList = unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            orderHeaderList = unitOfWork.OrderHeader
                .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
        }
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
