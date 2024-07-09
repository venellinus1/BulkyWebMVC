﻿using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class ShoppingCartController
    (IUnitOfWork unitOfWork)
    : Controller
{    
    public ShoppingCartVM ShoppingCartVM { get; set; }
    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartVM = new()
        {
            ShoppingCartList = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
        };
        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPriceBasedOnQuantity(cart);
            ShoppingCartVM.OrderTotal += (cart.Price * cart.Count);
        }
        return View(ShoppingCartVM);
    }
    public IActionResult Summary ()
    {
        return View();
    }
    public IActionResult Plus(int cartId)
    {
        var cartFromDb = unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        cartFromDb.Count += 1;
        unitOfWork.ShoppingCart.Update(cartFromDb);
        unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Minus(int cartId)
    {
        var cartFromDb = unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        if (cartFromDb.Count <= 1)
        {
            //remove item from cart
            unitOfWork.ShoppingCart.Remove(cartFromDb);
        } else
        {
            cartFromDb.Count -= 1;
            unitOfWork.ShoppingCart.Update(cartFromDb);
        }
        
        unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var cartFromDb = unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        unitOfWork.ShoppingCart.Remove(cartFromDb);
        unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }
        else
        {
            if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }
}
