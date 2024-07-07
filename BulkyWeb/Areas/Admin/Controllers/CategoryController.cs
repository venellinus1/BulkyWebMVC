using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticDetails.Role_Admin)]
public class CategoryController(IUnitOfWork unitOfWork)
    : Controller
{
    public IActionResult Index()
    {
        var categoryList = unitOfWork.Category.GetAll().ToList();
        return View(categoryList);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (category.Name == category.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name", "Display order cannot match tne Category name");
        }

        if (ModelState.IsValid)
        {
            unitOfWork.Category.Add(category);
            unitOfWork.Save();
            TempData["success"] = "Category created successfully";
            return RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        Category? categoryFromDb = unitOfWork.Category.Get(c => c.Id == id);

        if (categoryFromDb == null)
            return NotFound();

        return View(categoryFromDb);
    }
    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (category.Name == category.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name", "Display order cannot match tne Category name");
        }

        if (ModelState.IsValid)
        {
            unitOfWork.Category.Update(category);
            unitOfWork.Save();
            TempData["success"] = "Category updated successfully";
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
        Category? categoryFromDb = unitOfWork.Category.Get(c => c.Id == id);

        if (categoryFromDb == null)
            return NotFound();

        return View(categoryFromDb);
    }
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        Category? category = unitOfWork.Category.Get(c => c.Id == id);
        if (category == null)
            return NotFound();
        unitOfWork.Category.Remove(category);
        unitOfWork.Save();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}
