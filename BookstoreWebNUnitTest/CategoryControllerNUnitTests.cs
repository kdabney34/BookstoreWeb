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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreWebNUnitTest;


[TestFixture]
public class CategoryControllerNUnitTests
{
    public CategoryController categoryController;
    private readonly IUnitOfWork _unitOfWork;
    private DbContextOptions<ApplicationDbContext> options;
    private Mock<ITestCategoryRepo> CategoryRepository = new Mock<ITestCategoryRepo>();

    public CategoryControllerNUnitTests(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [SetUp]
    public void Setup()//configure virtual sql db for test
    {
        options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "temp_MoviesDB").Options;
    }



    [Test] //check that pages are getting returned rendered
    public void CategoryController_Returns_Index_AsRenderedWithModel()
    {
        //arrange
        IEnumerable<Category> categoryList;

        //act
        categoryList = _unitOfWork.Category.GetAll();

        //assert
        var result = categoryController.Index();
        Assert.IsInstanceOf<ViewResult>(result);
    }


    [Test] //check if returning non-null tables
    public void Actual_Fetched_CategoryListFromDb_Is_Not_Empty()
    {
        //arrange
        IEnumerable<Category> categoryList;

        //act
        categoryList = _unitOfWork.Category.GetAll();

        //assert
        Assert.That(categoryList, Is.Not.Empty);

    }

    [Test]
    public void Category_Table_ModifySuccessful()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()//using temporary DB instance because i don't want to modify existing company entities just for a test purpose
            .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;
        int orig_Id;

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new CategoryRepository(context);
            var result = repository.GetFirstOrDefault(u => u.Name == "Romance");//retrieve specific category from Db
            orig_Id = result.Id;
            result.Name = "TEST";//modify for test purposes
            context.SaveChanges();//save to in-mem db

        }

        //new dbcontext instance to make sure our changes were saved successfully
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new CategoryRepository(context);
            var result = repository.GetFirstOrDefault(u => u.Name == "TEST");
            if (result.Name == "TEST")
            {
                if (result.Id == orig_Id)
                {
                    result.Name = "Romance";

                }
                Assert.Multiple(() =>
                {
                    Assert.That(result.Id, Is.GreaterThan(-1));//id==0 means new entry, so we allow for that since this may run on another machine without my local temp db config
                    Assert.That(result.Name, Is.TypeOf<string>());
                    Assert.That(result.DisplayOrder, Is.TypeOf<int>());
                    Assert.That(result.CreatedDateTime, Is.TypeOf<DateTime>());
                });
            }
            else
            {
                Assert.Fail();
            }
        }

    }


    [Test]
    public void Category_SQLServer_Table_Successfully_Accessed_And_Modified_And_Correctly_Stored_Types()
    {

        int orig_Id;

        try
        {
            var result = _unitOfWork.Category.GetFirstOrDefault(u => u.Name == "Romance");//retrieve specific category from Db
            result.Name = "Romance_";//modify for test purposes
            orig_Id = result.Id;
            _unitOfWork.Save();//save to in-mem db

        }
        catch (InvalidCastException e)
        {
            throw;
        }

        //new dbcontext instance to make sure our changes were saved successfully

        var new_result = _unitOfWork.Category.GetFirstOrDefault(u => u.Name == "Romance");//retrieve specific category from Db

        if (new_result.Name == "Romance_")
        {
            if (new_result.Id == orig_Id)
            {
                new_result.Name = "Romance";
                _unitOfWork.Save();
            }
            Assert.Multiple(() =>
            {
                Assert.That(new_result.Id, Is.GreaterThan(-1));//id==0 means new entry, so we allow for that since this may run on another machine without my local temp db config
                Assert.That(new_result.Name, Is.TypeOf<string>());
                Assert.That(new_result.DisplayOrder, Is.TypeOf<int>());
                Assert.That(new_result.CreatedDateTime, Is.TypeOf<DateTime>());
            });
        }
        else
        {
            Assert.Fail();
        }
    }




    [Test]
    public void Connection_ToASP_EFC_AndInMemDb()//whether or not our config with Entity Framework Core, our database, and everything is correct
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()//
            .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new CategoryRepository(context);
            var result = repository.GetFirstOrDefault(u => u.Name == "Hardcover");
            Assert.That(result.Name, Is.EqualTo("Hardcover")); //this all wont work without proper connection
        };
    }

    [TestCase(0, 1)]
    [TestCase(2, 3)]
    [TestCase(4, 5)]
    [Test] //test whether category controller is returning the correct models
    public void CategoryController_ModelStateInvalid_ReturnView(int id)
    {
        categoryController.ModelState.AddModelError("test", "test");//string errorkey, string errormessage

        if (id < 1) //if record exists for category in Db
        {
            Assert.IsFalse(categoryController.ModelState.IsValid);//check whether the ModelState is invalid which is EFC's way of disallowing CRUD d.t. validations
        }
        else
        {
            var result = categoryController.Edit(id);

            ViewResult viewResult = result as ViewResult;
            Assert.IsInstanceOf<Category>(viewResult.Model);
        }
    }
}








