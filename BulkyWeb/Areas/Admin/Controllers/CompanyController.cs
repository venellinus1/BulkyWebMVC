using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices.Marshalling;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticDetails.Role_Admin)]
public class CompanyController(IUnitOfWork unitOfWork)
    : Controller
{
    public IActionResult Index()
    {
        var companyList = unitOfWork.Company.GetAll().ToList();        
        return View(companyList);
    }

    public IActionResult Upsert(int? id)
    {
        if (id == null || id == 0)
        {
            //create
			return View(new Company());
		}
        else
        {
            //update
            Company company = unitOfWork.Company.Get(p => p.Id == id);
            return View(company);
        }
    }
    [HttpPost]
    public IActionResult Upsert(Company company)
    {        
        if (ModelState.IsValid)
        {    
            if (company.Id == 0)
            {
				unitOfWork.Company.Add(company);
			}
            else
            {
				unitOfWork.Company.Update(company);
			}
			
            unitOfWork.Save();
            TempData["success"] = "Company created successfully";
            return RedirectToAction("Index");
        }
        else
        {            
			return View(company);
		}
        
    }

    
    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Company> objCompanyList = unitOfWork.Company.GetAll().ToList();
        return Json(new { data = objCompanyList });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var companyToDelete = unitOfWork.Company.Get(p => p.Id == id);
        if(companyToDelete == null)
        {
            return Json(new { success = false, message = "Error while deleting"});
        }
        unitOfWork.Company.Remove(companyToDelete);
        unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }
    #endregion
}
