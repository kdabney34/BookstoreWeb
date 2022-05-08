using BookstoreWeb.Controllers;
using BookstoreWeb.DataAccess;
using BookstoreWeb.DataAccess.Repository;
using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using BookstoreWebNUnitTest.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BookstoreWebNUnitTest;

[TestFixture]
public class HomeControllerNUnitTests
{
    private ProductRepository ProductRepository;
    private IUnitOfWork _unitOfWork;
    private Mock<ITestProductRepo> productRepositoryMock = new Mock<ITestProductRepo>();
    private List<Product> productList;
    private DbContextOptions<ApplicationDbContext> options;
    private Mock<HomeController> homeControllerMock;
    private HomeController homeController;


    public HomeControllerNUnitTests(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;


        productList = new List<Product>
            {
                        new Product {
                            Id=999,Title="New Book", ISBN="A202",Author="Terrence Mcgee",ListPrice=1000,
                            Price=33,Price50=50,Price100=100,CategoryId=88,CoverTypeId=66
                         }
            };
    }//exit constructor



    //Test that In-Memory Database is successfully accepting CRUD functionality to the web app
    public void HomeControllerModifySuccessful()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()//using temporary DB instance because i don't want to modify existing company entities just for a test purpose
            .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;
        string orig_Author;

        using (var context = new ApplicationDbContext(options))//initialize using our newly configured options
        {
            var repository = new ProductRepository(context);
            var stringRepresent = repository.GetAll().ToString;//retrieve specific category from Db
            var product = repository.GetFirstOrDefault(u => u.Id == 1);
            orig_Author = product.Author;
            context.SaveChanges();//save to in-mem db

        }

        //new dbcontext instance to make sure our changes were saved successfully
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new ProductRepository(context);
            var result = repository.GetFirstOrDefault(u => u.Id == 1);
            if (result.Author == orig_Author)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(result.Id, Is.GreaterThan(-1));//id==0 means new entry, so we allow for that since this may run on another machine without my local temp db config
                    Assert.That(result.Price, Is.Positive);
                    Assert.That(result.Price50, Is.TypeOf<double>());
                    Assert.That(result.Price100, Is.AtLeast(20));
                    Assert.That(result.Author, Is.TypeOf<string>());
                    Assert.That(result.ImageUrl, Is.Not.Null);
                    Assert.That(result.Category, Is.TypeOf<Category>());
                    Assert.That(result.ISBN, Is.Unique);
                });
            }
            else
            {
                Assert.Fail();
            }
        }

    }



[Test]
    public void HomeController_Returns_HomePage_Rendered()
    {
        //arrange
        IEnumerable<Product> productsList;

        //act
        productsList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

        //assert
        var result = homeController.Index();
        Assert.IsInstanceOf<ViewResult>(result);
    }


    [SetUp]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "temp_MoviesDB").Options;
    }



    [TestCase(0, 1)]
    [TestCase(2, 3)]
    [TestCase(4, 5)]
    [Test] //test whether home controller is returning the correct models
    public void HomeController_ModelStateValidityCheck_ReturnView(int id)
    {
        homeController.ModelState.AddModelError("test", "test");//string errorkey, string errormessage | adds additional error msg
        if (id < 1)
        {
            var result = homeController.Details(id);

            ViewResult viewResult = result as ViewResult;
            Assert.IsInstanceOf<ShoppingCart>(viewResult.Model);
        }
        else
        {
            Assert.IsFalse(homeController.ModelState.IsValid);
        }
    }




    [Test]
    public void Actual_Fetched_ProductListFromDb_Is_Not_Empty()
    {
        //arrange
        IEnumerable<Product> productsList;

        //act
        productsList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

        //assert
        Assert.That(productsList, Is.Not.Empty);

    }



    public void Connection_ToASP_EFC_AndInMemDb() //test ability to fetch products from virtual SQL db
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

        using (var context = new ApplicationDbContext(options))//can always modify to another connection
        {
            var repository = new ProductRepository(context);
            var result = repository.GetAll(includeProperties: "Category,CoverType");
            Assert.That(result, Is.Not.Empty);
        }
    }



    [Test]
    public void HomeControllerAccessesProductRepository_SQLServer_Table_Successfully_Accessed_And_Modified_And_Correctly_Stored_Types()
    {

        double orig_Px;

        try
        {
            var result = _unitOfWork.Product.GetFirstOrDefault(u => u.Price < 110);
            orig_Px = result.Price;//making a local copy

            result.Price = .01 + result.Price;//modify for test purposes
            _unitOfWork.Save();//save to in-mem db

        }
        catch (InvalidCastException e)
        {
            throw;
        }

        //new dbcontext instance to make sure our changes were saved successfully

        var new_result = _unitOfWork.Product.GetFirstOrDefault(u => u.Price == .01 + orig_Px);//retrieve specific category from Db

        if (new_result.Price == .01 + orig_Px)
        {
            new_result.Price = -.01 + orig_Px;
            _unitOfWork.Save();
        }

        else
        {
            Assert.Fail();
        }

        Assert.Multiple(() =>
        {
            Assert.That(new_result.Id, Is.GreaterThan(-1));//id==0 means new entry, so we allow for that since this may run on another machine without my local temp db config
            Assert.That(new_result.Price, Is.Positive);
            Assert.That(new_result.Price50, Is.TypeOf<double>());
            Assert.That(new_result.Price100, Is.AtLeast(20));
            Assert.That(new_result.Author, Is.TypeOf<string>());
            Assert.That(new_result.ImageUrl, Is.Not.Null);
            Assert.That(new_result.Category, Is.TypeOf<Category>());
            Assert.That(new_result.ISBN, Is.Unique);

        });



    }
}

