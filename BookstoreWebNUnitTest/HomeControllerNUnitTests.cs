using BookstoreWeb.Controllers;
using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace BookstoreWebNUnitTest
{
    public class HomeControllerNUnitTests
    {
        private HomeController homeController;
        //private Mock<IUnitOfWork> _unitOfWork;
        private Mock<ITestProductRepo> ProductRepository = new Mock<ITestProductRepo>();
        private IEnumerable<Product> productList;

        [SetUp]
        public void Setup()
        {
            productList = new List<Product> {
                new Product{
                    Id=999,Title="New Book", ISBN="A202",Author="Terrence Mcgee",ListPrice=1000,
                    Price=33,Price50=50,Price100=100,CategoryId=88,CoverTypeId=66
                }
            };
            ProductRepository.Setup(x => x.GetAll()).Returns(true);

        }


        //TEST THAT UNITOFWORK.PRODUCT.GETALL() IS INVOKED WITH ITS OPTIONS CORRECTLY
        //        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
        // => return View(productList)
        [Test]
        public void Entire_productList_Acquired_Successfully()
        {
        }
    }
}