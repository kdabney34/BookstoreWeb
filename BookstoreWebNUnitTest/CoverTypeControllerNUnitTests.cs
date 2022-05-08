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
public class CoverTypeControllerNUnitTests
{
    private CoverType CoverType_1;
    private CoverType CoverType_2;
    private HomeController homeController;
    private IUnitOfWork _unitOfWork;
    private Mock<ITestCoverTypeRepo> CoverTypeRepository = new Mock<ITestCoverTypeRepo>();
    private List<CoverType> coverList;
    private DbContextOptions<ApplicationDbContext> options;
    private Mock<CoverTypeController> covertypeControllerMock;
    private CoverTypeController coverTypeController;

    public CoverTypeControllerNUnitTests(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        coverList = new List<CoverType>
            {
                        new CoverType {
                            Name = "Tony Simmons"
                         },
                        new CoverType {
                            Name = "Albert Dean"
                         }
            };
    }//exit constructor


    [TestCase(0, 1)]
    [TestCase(2, 3)]
    [TestCase(4, 5)]
    [Test] //test whether CoverType controller is returning the correct models
    public void CoverTypeController_ModelStateValidityCheck_ReturnView(int id)
    {
        homeController.ModelState.AddModelError("test", "test");//string errorkey, string errormessage | adds error bound to original
        if (id < 1)
        {
            var result = coverTypeController.Edit(id);

            ViewResult viewResult = result as ViewResult;
            Assert.IsInstanceOf<CoverType>(viewResult.Model);
        }
        else
        {
            Assert.IsFalse(homeController.ModelState.IsValid);//in prod i would make a MOQ of CTcontroller and use in homeController's place
        }
    }



    [Test]
    public void CoverType_SQLServer_Table_Successfully_Accessed_And_Modified_And_Correctly_Stored_Types()
    {

        int orig_Id;

        try
        {
            var result = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Name == "Softcover");//these tests need the local in-mem db to run
            result.Name = "Softcover_";//modify for test purposes
            orig_Id = result.Id;//making a local copy
            _unitOfWork.Save();//save to in-mem db

        }
        catch (InvalidCastException e)
        {
            throw;
        }

        //new dbcontext instance to make sure our changes were saved successfully

        var new_result = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Name == "Softcover_");//retrieve specific category from Db

        if (new_result.Name == "Softcover_")
        {
            if (new_result.Id == orig_Id)
            {
                new_result.Name = "Softcover";
                _unitOfWork.Save();
            }
            Assert.Multiple(() =>
            {
                Assert.That(new_result.Id, Is.GreaterThan(-1));//id==0 means new entry, so we allow for that since this may run on another machine without my local temp db config
                Assert.That(new_result.Name, Is.TypeOf<string>());
            });
        }
        else
        {
            Assert.Fail();
        }
    }




    public void CoverType_Table_ModifySuccessful()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()//using temporary DB instance because i don't want to modify existing company entities just for a test purpose
            .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;
        int orig_Id;

        using (var context = new ApplicationDbContext(options))//initialize using our newly configured options
        {
            var repository = new CoverTypeRepository(context);
            var result = repository.GetFirstOrDefault(u => u.Name == "Hardcover");//retrieve specific category from Db
            orig_Id = result.Id;
            result.Name = "TEST";//modify for test purposes
            context.SaveChanges();//save to in-mem db

        }

        //new dbcontext instance to make sure our changes were saved successfully
        using (var context = new ApplicationDbContext(options))
        {
            var repository = new CoverTypeRepository(context);
            var result = repository.GetFirstOrDefault(u => u.Name == "TEST");
            if (result.Id == orig_Id)
            {
                result.Name = "Romance";

            }
            else
            {
                Assert.Fail();
            }
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.GreaterThan(-1));//id==0 means new entry, so we allow for that since this may run on another machine without my local temp db config
                Assert.That(result.Name, Is.TypeOf<string>());
            });

        }

    }

    [Test]
    public void Actual_Fetched_CoverTypeListFromDb_Is_Not_Empty()
    {
        //arrange
        IEnumerable<Category> categoryList;

        //act
        categoryList = _unitOfWork.Category.GetAll();

        //assert
        Assert.That(categoryList, Is.Not.Empty);

    }

    [Test]
    public void CategoryController_Returns_Index_RenderedWithModel()
    {
        //arrange
        IEnumerable<Product> productsList;

        //act
        productsList = _unitOfWork.Product.GetAll();

        //assert
        var result = homeController.Index();
        Assert.IsInstanceOf<ViewResult>(result);
    }


    [SetUp]//configure virtual database for test
    public void Setup()
    {
        options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;
    }

    public void Connection_ToASP_EFC_AndInMemDb() //test ability to fetch covertypes from virtual SQL db
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

        using (var context = new ApplicationDbContext(options))//can always modify to another connection
        {
            var repository = new CoverTypeRepository(context);
            var result = repository.GetFirstOrDefault(u => u.Name == "Hardcover");
            Assert.That(result.Name, Is.EqualTo("Hardcover"));
        };
    }
}
