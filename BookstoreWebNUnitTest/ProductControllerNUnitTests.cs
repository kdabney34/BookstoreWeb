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

namespace BookstoreWebNUnitTest
{

    [TestFixture]
    public class ProductControllerNUnitTests
    {
        public HomeController homeController;
        private readonly IUnitOfWork _unitOfWork;
        private DbContextOptions<ApplicationDbContext> options;
        private ProductRepository productRepository;

        public ProductControllerNUnitTests(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }//exit ctor


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
            IEnumerable<Product> productList;

            //act
            productList = _unitOfWork.Product.GetAll();

            //assert
            Assert.That(productList, Is.Not.Empty);
        }

        [Test]
        public void ProductController_Returns_Index_RenderedWithModel()
        {
            //arrange
            IEnumerable<Product> productList;

            //act
            productList = _unitOfWork.Product.GetAll();

            //assert
            var result = productController.Index();
            Assert.IsInstanceOf<ViewResult>(result);
        }


        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "temp_MoviesDB").Options;
        }

        public void Virtual_Category_SQLDB_ExtractTest() //test ability to fetch products from virtual SQL db
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

            using (var context = new ApplicationDbContext(options))//can always modify to another connection
            {
                var repository = new ProductRepository(context);
                var result = repository.GetAll(includeProperties:"Category,CoverType");
                Assert.That(result, Is.Not.Empty);
            };
        }
    }
}







