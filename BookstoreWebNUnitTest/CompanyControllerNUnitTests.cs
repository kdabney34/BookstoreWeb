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
    public class CompanyControllerNUnitTests
    {
        public CompanyController companyController;
        private readonly IUnitOfWork _unitOfWork;
        private DbContextOptions<ApplicationDbContext> options;


        public CompanyControllerNUnitTests(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [SetUp]
        public void Setup()//configure virtual sql db for test
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "temp_MoviesDB").Options;
        }



        [Test]
        public void Company_SQLServer_Table_Successfully_Accessed_And_Modified_And_Correctly_Stored_Types()
        {

            int orig_Id;

            try
            {
                var result = _unitOfWork.Company.GetFirstOrDefault(u => u.Name == "Luke Metals LLC");//these tests need the local in-mem db to run
                result.Name = "Luke Metals";//modify for test purposes
                orig_Id = result.Id;//making a local copy
                _unitOfWork.Save();//save to in-mem db

            }
            catch (InvalidCastException e)
            {
                throw;
            }

            //new dbcontext instance to make sure our changes were saved successfully

            var new_result = _unitOfWork.Company.GetFirstOrDefault(u => u.Name == "Luke Metals");//retrieve specific category from Db

            if (new_result.Name == "Luke Metals")
            {
                if (new_result.Id == orig_Id)
                {
                    new_result.Name = "Luke Metals LLC";
                    _unitOfWork.Save();
                }
                Assert.Multiple(() =>
                {
                    Assert.That(new_result.Id, Is.GreaterThan(-1));//id==0 means new entry, so we allow for that since this may run on another machine without my local temp db config
                    Assert.That(new_result.Name, Is.TypeOf<string>());
                    Assert.That(new_result.StreetAddress, Is.Positive);
                    Assert.That(new_result.City, Is.Not.StringContaining("Dallas"));
                    Assert.That(new_result.State, Is.Not.AnyOf("TX","IA","VT","MA","MI","KS"));
                    Assert.That(new_result.PostalCode, Is.AtLeast(32));
                    Assert.That(new_result.PhoneNumber, Is.TypeOf<string>());

                });
            }
            else
            {
                Assert.Fail();
            }
        }


        [TestCase(0, 1)]
        [TestCase(2, 3)]
        [TestCase(4, 5)]
        [Test] //test whether company controller is returning the correct models
        public void CompanyController_ModelStateValidityCheck_ReturnView(int id)
        {
            companyController.ModelState.AddModelError("test", "test");//string errorkey, string errormessage
            if (id < 1)
            {
                ShoppingCart cartObj = new()
                {
                    Count = 1,
                    Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == 1, includeProperties: "Category,CoverType"),
                };

                var result = companyController.Upsert(id);

                ViewResult viewResult = result as ViewResult;
                Assert.IsInstanceOf<Category>(viewResult.Model);
            }
            else
            {
                Assert.IsFalse(companyController.ModelState.IsValid);
            }
        }



        [Test] //check that pages are getting returned rendered
        public void CompanyController_Returns_Index_AsRenderedWithModel()
        {
            //arrange
            IEnumerable<Company> companyList;

            //act
            companyList = _unitOfWork.Company.GetAll();

            //assert
            var result = companyController.Index();
            Assert.IsInstanceOf<ViewResult>(result);
        }

        public void Connection_ToASP_EFC_AndInMemDb() //test ability to fetch all Company info from virtual SQL db
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

            using (var context = new ApplicationDbContext(options))//can always modify to another connection
            {
                var repository = new CompanyRepository(context);
                var result = repository.GetAll();
                Assert.That(result, Is.Not.Empty);
            };
        }


        [Test] //check if returning non-null tables
        public void Actual_Fetched_CompanyListFromDb_Is_Not_Empty()
        {
            //arrange
            IEnumerable<Company> companyList;

            //act
            companyList = _unitOfWork.Company.GetAll();

            //assert
            Assert.That(companyList, Is.Not.Empty);

        }


        public void Company_Table_ModifySuccessful()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()//using temporary DB instance because i don't want to modify existing company entities just for a test purpose
                .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new CompanyRepository(context);
                var result = repository.GetFirstOrDefault(u => u.Name == "Applied Materials, Inc.");//retrieve specific company from Db
                result.State = "TEST";
                context.SaveChanges();
            }

            //new dbcontext instance to make sure our changes were saved successfully
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new CompanyRepository(context);
                var result = repository.GetFirstOrDefault(u => u.Name == "Applied Materials, Inc.");//retrieve specific company from Db
                if (result.State == "TEST")
                {
                    result.State = "TX";
                    Assert.Multiple(() =>
                    {
                        Assert.That(result.Id, Is.GreaterThan(-1));//id==0 means new entry, so we allow for that since this may run on another machine without my local temp db config
                        Assert.That(result.City, Is.EqualTo("Austin"));
                        Assert.That(result.PostalCode, Is.Positive);
                        Assert.That(result.StreetAddress, Is.Unique);
                        Assert.That(result.PhoneNumber, Is.TypeOf<string>());
                        Assert.That(result.Name, Is.EqualTo("Applied Materials, Inc."));
                    });
                }
                else
                {
                    Assert.Fail();
                }
            }

        }
    };
}






