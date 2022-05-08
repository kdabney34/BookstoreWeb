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

    public void Virtual_CoverType_SQLDB_ExtractTest () //test ability to fetch covertypes from virtual SQL db
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
    