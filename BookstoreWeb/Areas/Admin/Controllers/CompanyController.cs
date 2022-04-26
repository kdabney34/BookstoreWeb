using BookstoreWeb.DataAccess;
using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using BookstoreWeb.Models.ViewModels;
using BookstoreWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BookstoreWeb.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]//all crud operations can only be done by admin or employee

public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;


    public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }

    //GET
    public IActionResult Upsert(int? id)
    {
        Company company = new();
        
        if (id == null || id == 0)
        {
            //create product
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CoverTypeList"] = CoverTypeList;
            return View(company);
        }
        else
        {
            company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            return View(company);

            //update product
        }


    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company obj)
    {

        if (ModelState.IsValid)
        {
            
            if (obj.Id == 0)
            {
                TempData["success"] = "Company Added Successfully";
                _unitOfWork.Company.Add(obj);
            }
            else
            {
                TempData["success"] = "Company Updated Successfully";
                _unitOfWork.Company.Update(obj);
            }
            _unitOfWork.Save();
            
            return RedirectToAction("Index");
        }
        return View(obj);
    }



    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var companyList = _unitOfWork.Company.GetAll();
        return Json(new { data = companyList });
    }

    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfWork.Company.Remove(obj);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });

    }
    #endregion
}
