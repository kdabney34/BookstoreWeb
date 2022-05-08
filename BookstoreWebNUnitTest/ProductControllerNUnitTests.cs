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
        public ProductController productController;
        private readonly IUnitOfWork _unitOfWork;
        private DbContextOptions<ApplicationDbContext> options;

        public ProductControllerNUnitTests(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }//exit ctor

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







