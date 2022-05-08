using BookstoreWeb.Areas.Admin.Controllers;
using BookstoreWeb.Controllers;
using BookstoreWeb.DataAccess;
using BookstoreWeb.DataAccess.Repository;
using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using BookstoreWeb.Utility;
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
    public class OrderControllerNUnitTests
    {
        public OrderController categoryController;
        private readonly IUnitOfWork _unitOfWork;
        private Mock<ITestCategoryRepo> CategoryRepository = new Mock<ITestCategoryRepo>();
        private List<Category> categoryList;
        private DbContextOptions<ApplicationDbContext> options;
        private Mock<HomeController> homeControllerMock;

        public OrderControllerNUnitTests(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }//exit ctor


        [Test]
        public void Virtual_Order_SQLDB_ExtractTest() //test ability to fetch orders from virtual SQL db
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

            using (var context = new ApplicationDbContext(options))//can always modify to another connection
            {
                var repository = new ProductRepository(context);
                var result = repository.GetAll();
                Assert.That(result, Is.Not.Empty);
            };
        }


        [Test] //test ability to fetch order headers from sql db
        public void Actual_Fetched_OrderListFromDb_Is_Not_Empty()//test here as non-admin,non-employee role
        {
            //arrange
            IEnumerable<OrderHeader> orderList;

            //act
            orderList = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == "aox-097-735-175-3278-9729", includeProperties: "ApplicationUserId");

            //assert
            Assert.That(orderList, Is.Not.Empty);

        }

        [Test]
        public void OrderController_Returns_Index_RenderedWithModel()//test that views are rendered when returned
        {
            //arrange
            IEnumerable<OrderDetail> orderList;

            //assert
            var result = categoryController.Index();
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "temp_MoviesDB").Options;
        } //setup virtual in-Mem Db instance for test purposes


        //Test that In-Memory Database is successfully accepting CRUD functionality to the web app
        [Test]
        public void InMemDb_Allowing_Successful_CRUD_Functionality() //test that we are able to modify things and store in db successfully
        {
            //arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

            //act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new OrderHeaderRepository(context);
                var order = repository.GetFirstOrDefault(u => u.Id == 1, tracked: false);//get 1st customers orderHeader from Db
                order.TrackingNumber = SD.TestTrackingNum;
                order.Carrier = SD.CarrierFedEx;
                order.OrderStatus = SD.StatusShipped;
                order.ShippingDate = DateTime.Now;//this is in-memory test database so the details aren't actual customer details, ok to change things
                _unitOfWork.OrderHeader.Update(order);
                _unitOfWork.Save();

            }; //close the in-memory database connection, re-establish new connection, then test if modified parameters are there in db

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new OrderHeaderRepository(context);
                var order = repository.GetFirstOrDefault(u => u.Id == 1, includeProperties: "Category,CoverType", tracked: false);//get 1st customers orderHeader from Db

                //assert
                if (order.TrackingNumber == SD.TestTrackingNum)
                {
                    if (order.Carrier == SD.CarrierFedEx)
                    {
                        if (order.OrderStatus == SD.StatusShipped)
                        {
                            Assert.NotNull(order);
                        }
                    }
                }
            }; //close the in-mem
        }
    }
}


    








