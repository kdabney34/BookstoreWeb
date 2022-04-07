using BookstoreWeb.DataAccess;
using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using BookstoreWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BookstoreWeb.Controllers;

public class CoverTypeController : Controller
{
    private readonly IUnitOfWork _unitofwork;

    public CoverTypeController(IUnitOfWork unitofwork)
    {
        _unitofwork = unitofwork ;
    }

    public IActionResult Index()
    {
        IEnumerable<CoverType> objCategoryList = _unitofwork.CoverTypes.GetAll(); 
        return View(objCategoryList);
    }

    // GET from db
    public IActionResult Create()
    {
        return View();
    }

    // POST to db
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CoverType obj)
    {
        if (ModelState.IsValid)
        {
            _unitofwork.CoverTypes.Add(obj);
            _unitofwork.Save();
            TempData["success"] = "Category created successfully!";
            return RedirectToAction("Index");
        }
        return View(obj);
    }


    // GET from db
    public IActionResult Edit(int? id)
    {
        if (id == 0 || id == null)
        {
            return NotFound();
        }
        // var categoryFromDb = _db.Categories.Find(id); 
        var categoryFromDb = _unitofwork.CoverTypes.GetFirstOrDefault(u => u.Id == id);
        return View(categoryFromDb);
    }

    // POST to db
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CoverType obj)
    {
        if (ModelState.IsValid)
        {
            _unitofwork.CoverTypes.Update(obj);
            _unitofwork.Save();
            TempData["success"] = "Category edited successfully!";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    //\\ GET DELETE PAGE
    public IActionResult Delete(int? id)
    {
        var obj = _unitofwork.CoverTypes.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }
        return View(obj);
    }

    //\\ POST DELETED ROW 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePOST(int? id)
    {
        var obj = _unitofwork.CoverTypes.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }
        _unitofwork.CoverTypes.Remove(obj); //Covertypes == repo , covertype == model
        _unitofwork.Save();
        TempData["success"] = "Category deleted successfully!";
        return RedirectToAction("Index");
    }
}
