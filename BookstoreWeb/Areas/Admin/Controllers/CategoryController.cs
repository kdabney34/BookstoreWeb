using BookstoreWeb.DataAccess;
using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using BookstoreWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BookstoreWeb.Controllers;

public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitofwork;

    public CategoryController(IUnitOfWork unitofwork)
    {
        _unitofwork = unitofwork ;
    }

    public IActionResult Index()
    {
        IEnumerable<Category> objCategoryList = _unitofwork.Category.GetAll(); 
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
    public IActionResult Create(Category obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the name");
        }
        if (ModelState.IsValid)
        {
            _unitofwork.Category.Add(obj);
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
        var categoryFromDb = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);
        return View(categoryFromDb);
    }

    // POST to db
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the name");
        }
        if (ModelState.IsValid)
        {
            _unitofwork.Category.Update(obj);
            _unitofwork.Save();
            TempData["success"] = "Category edited successfully!";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    //\\ GET DELETE PAGE
    public IActionResult Delete(int? id)
    {
        var obj = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);
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
        var obj = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return NotFound();
        }
        _unitofwork.Category.Remove(obj);
        _unitofwork.Save();
        TempData["success"] = "Category deleted successfully!";
        return RedirectToAction("Index");
    }
}
