using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController(IUnitOfWork unitOfWork)
    : Controller
{
    public IActionResult Index()
    {
        var productList = unitOfWork.Product.GetAll().ToList();        
        return View(productList);
    }

    public IActionResult Create()
    {
        ProductVM productVM = new() 
        {
            CategoryList = unitOfWork.Category
            .GetAll()
            .Select(cat => new SelectListItem
			{
				Text = cat.Name,
				Value = cat.Id.ToString()
			}),
            Product = new Product()
        };
		return View(productVM);
    }
    [HttpPost]
    public IActionResult Create(ProductVM productViewModel)
    {        
        if (ModelState.IsValid)
        {
            unitOfWork.Product.Add(productViewModel.Product);
            unitOfWork.Save();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }
        else
        {
            productViewModel.CategoryList = unitOfWork.Category
            .GetAll()
            .Select(cat => new SelectListItem
            {
                Text = cat.Name,
                Value = cat.Id.ToString()
            });
			return View(productViewModel);
		}
        
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        Product? productFromDb = unitOfWork.Product.Get(c => c.Id == id);

        if (productFromDb == null)
            return NotFound();

        return View(productFromDb);
    }
    [HttpPost]
    public IActionResult Edit(Product product)
    {        
        if (ModelState.IsValid)
        {
            unitOfWork.Product.Update(product);
            unitOfWork.Save();
            TempData["success"] = "Product updated successfully";
            return RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        Product? productromDb = unitOfWork.Product.Get(c => c.Id == id);

        if (productromDb == null)
            return NotFound();

        return View(productromDb);
    }
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        Product? product = unitOfWork.Product.Get(c => c.Id == id);
        if (product == null)
            return NotFound();
        unitOfWork.Product.Remove(product);
        unitOfWork.Save();
        TempData["success"] = "Product deleted successfully";
        return RedirectToAction("Index");
    }
}
