﻿using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController(ApplicationDBContext db)
    : Controller
{
    public IActionResult Index()
    {
        var categoryList = db.Categories.ToList();
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
			db.Categories.Add(category);
			db.SaveChanges();
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
		Category? categoryFromDb = db.Categories.Find(id);		

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
			db.Categories.Update(category);
			db.SaveChanges();
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
		Category? categoryFromDb = db.Categories.Find(id);

		if (categoryFromDb == null)
			return NotFound();

		return View(categoryFromDb);
	}
	[HttpPost, ActionName("Delete")]
	public IActionResult DeletePost(int? id)
	{
		Category? category = db.Categories.Find(id);
		if (category == null)
			return NotFound();
		db.Categories.Remove(category);
		db.SaveChanges();
		TempData["success"] = "Category deleted successfully";
		return RedirectToAction("Index");
	}
}
