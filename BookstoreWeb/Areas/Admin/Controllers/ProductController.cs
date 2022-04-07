using BookstoreWeb.DataAccess;
using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using BookstoreWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookstoreWeb.Models.ViewModels;

namespace BookstoreWeb.Controllers;

public class ProductController : Controller
{
    private readonly IUnitOfWork _unitofwork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment _newwebhostenvironment)
    {
        _unitofwork = unitofwork ;
        _webHostEnvironment = _newwebhostenvironment ;
    }

    public IActionResult Index()
    { 
        return View();
    }

    // GET from db
    [HttpGet]
    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new()
        {
            product = new(), //this line is necessary else the Upsert view won't have access to product's properties despite calling ProductVM new() must say new() product for it to be usable and transferrable
            CategoryList = _unitofwork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            CoverTypeList = _unitofwork.CoverTypes.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
        };
        if (id == 0 || id == null)
        // omit but pay attention to below methods ::: ways to pass data              ***
        { // omitted in favor of above much more tightly-bound data structure            ***
            // ViewBag.CategoryList = CategoryList;
            // ViewData["CoverTypeList"] = CoverTypeList;
            return View(productVM);
        }
        else
        { // update product
            productVM.product = _unitofwork.Products.GetFirstOrDefault(u => u.Id == id);
            return View(productVM);
        }
    }

   // POST to db
   [HttpPost]
   [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductVM obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        { //{
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if  (file != null)
            {
                string filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(file.FileName);
                if (obj.product.ImageUrl != null) //only way this could be true is if the object's properties are already filled out before submitting post action
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.product.ImageUrl.TrimStart('\\'));
 
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var filestreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                {
                    file.CopyTo(filestreams);
                }
                obj.product.ImageUrl = @"\images\products\" + filename + extension ;
            }

            if (obj.product.Id == 0) { 
                _unitofwork.Products.Add(obj.product);
            }
            else {
                _unitofwork.Products.Update(obj.product);
            }
            _unitofwork.Save();
            TempData["success"] = "Product Edited Successfully!";
            return RedirectToAction("Index");
        }
        return View(obj);
    }


    ////\\ POST DELETED ROW 
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult DeletePOST(int? id)
    //{
    //    var obj = _unitofwork.Category.GetFirstOrDefault(u => u.Id == id);
    //    if (obj == null)
    //    {
    //        return NotFound();
    //    }
    //    _unitofwork.Category.Remove(obj);
    //    _unitofwork.Save();
    //    TempData["success"] = "Category deleted successfully!";
    //    return RedirectToAction("Index");
    //}


    #region API CALLS


    [HttpGet]
    public IActionResult GetAll()
    {
        var productlist = _unitofwork.Products.GetAll(includeProperties:"Category,CoverType");
        return Json(new { data = productlist });
        // API Endpoint
    }


    //\\ POST DELETED ROW 
    //POST
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var obj = _unitofwork.Products.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\')).Replace(@"\\", @"\");
        if (System.IO.File.Exists(oldImagePath))
        {
            System.IO.File.Delete(oldImagePath);
        }
        _unitofwork.Products.Remove(obj);
        _unitofwork.Save();
        return Json(new { success = true, message = "Delete Successful" });
        
        
        #endregion
    }

}
    

