using Bulky.Utility;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.ViewComponents;

public class ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            if (HttpContext.Session.GetInt32(StaticDetails.SessionCart) == null)
            {
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                    unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
            }            
            return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
        }
        else
        {
            HttpContext.Session.Clear();
            return View(0);
        }
    } 
}
