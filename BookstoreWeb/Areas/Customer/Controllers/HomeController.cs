using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using BookstoreWeb.Models.ViewModels;
using BookstoreWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace BookstoreWeb.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
       // ViewBag.isHome = true;

        return View(productList);
    }

    public IActionResult ShopIntro()
    {

        return View();
    }

    public IActionResult Gallery()
    {
        return View();
    }

    public IActionResult Details(int productId)
    {
        ShoppingCart cartObj = new()
        {
            Count=1,
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType"),
        };

        return View(cartObj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shoppingCart.ApplicationUserID = claim.Value;

        ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
            u=>u.ApplicationUserID==claim.Value && u.ProductID==shoppingCart.ProductID);

        if (cartFromDb==null)
        {
            _unitOfWork.ShoppingCart.Add(shoppingCart);
            _unitOfWork.Save(); //must use Save() to input to db before retrieving it as session integer in order for it to count
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claim.Value).ToList().Count);
        }                                                                   //retrieve all shopping carts(Order Details in db) and count them and store as int for session var
        else { _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            _unitOfWork.Save();
        }
       
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    //testing purposes to see if controller is accessible
    public string TestMethod()
    {
        return "test";
    }
}
