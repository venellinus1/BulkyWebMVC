using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    : Controller
{
    public IActionResult Index()
    {
        var productList = unitOfWork.Product.GetAll(includeProperties:"Category").ToList();        
        return View(productList);
    }

    public IActionResult Upsert(int? id)
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
        if (id == null || id == 0)
        {
            //create
			return View(productVM);
		}
        else
        {
            //update
            productVM.Product = unitOfWork.Product.Get(p => p.Id == id);
            return View(productVM);
        }
    }
    [HttpPost]
    public IActionResult Upsert(ProductVM productViewModel, IFormFile? file)
    {        
        if (ModelState.IsValid)
        {
            string wwwRootPath = webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");
                                
                if (!string.IsNullOrEmpty(productViewModel.Product.ImageUrl))
                {
                    //image url is present -> delete the existing image
                    var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                productViewModel.Product.ImageUrl = @"\images\product\" + fileName;
			}
            if (productViewModel.Product.Id == 0)
            {
                //adding new image
				unitOfWork.Product.Add(productViewModel.Product);
			}
            else
            {
				//updating existing image
				unitOfWork.Product.Update(productViewModel.Product);
			}
			
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
