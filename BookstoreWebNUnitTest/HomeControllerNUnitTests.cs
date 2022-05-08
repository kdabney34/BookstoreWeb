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
using System.Collections.Generic;

namespace BookstoreWebNUnitTest
{

    [TestFixture]
    public class HomeControllerNUnitTests
    {
        private HomeController homeController;
        private IUnitOfWork _unitOfWork;
        private Mock<ITestProductRepo> ProductRepository = new Mock<ITestProductRepo>();
        private List<Product> productList;
        private DbContextOptions<ApplicationDbContext> options;
        private Mock<HomeController> homeControllerMock;


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


        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "temp_MoviesDB").Options;
        }

        //TEST THAT UNITOFWORK.PRODUCT.GETALL() IS INVOKED WITH ITS OPTIONS CORRECTLY
        //        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
        // => return View(productList)
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

        public void Virtual_HomeProduct_SQLDB_ExtractTest() //test ability to fetch products from virtual SQL db
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

            using (var context = new ApplicationDbContext(options))//can always modify to another connection
            {
                var repository = new ProductRepository(context);
                var result = repository.GetAll(includeProperties: "Category,CoverType");
                Assert.That(result, Is.Not.Empty);
            };
        }

    }
}