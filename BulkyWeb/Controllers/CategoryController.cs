using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController(ICategoryRepository categoryRepository)
    : Controller
{
    public IActionResult Index()
    {
        var categoryList = categoryRepository.GetAll().ToList();
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
            categoryRepository.Add(category);
            categoryRepository.Save();
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
		Category? categoryFromDb = categoryRepository.Get(c => c.Id == id);		

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
            categoryRepository.Update(category);
            categoryRepository.Save();
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
		Category? categoryFromDb = categoryRepository.Get(c => c.Id == id);

		if (categoryFromDb == null)
			return NotFound();

		return View(categoryFromDb);
	}
	[HttpPost, ActionName("Delete")]
	public IActionResult DeletePost(int? id)
	{
		Category? category = categoryRepository.Get(c => c.Id == id);
		if (category == null)
			return NotFound();
        categoryRepository.Remove(category);
        categoryRepository.Save();
		TempData["success"] = "Category deleted successfully";
		return RedirectToAction("Index");
	}
}
