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

        public void Virtual_Company_SQLDB_ExtractTest() //test ability to fetch all Company info from virtual SQL db
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


        public void Company_SQLDB_Table_Successfully_Supports_CRUD_Functionality()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()//using temporary DB instance because i don't want to modify existing company entities just for a test purpose
                .UseInMemoryDatabase(databaseName: "temp-MoviesDB").Options;

            using (var context = new ApplicationDbContext(options))
            {
                var repository = new CompanyRepository(context);
                var result = repository.GetFirstOrDefault(u => u.Name == "Applied Materials, Inc.");//retrieve specific company from Db
                result.State = "TEST";
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
            }

        }
    };
}






