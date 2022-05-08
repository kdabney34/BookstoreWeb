using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookstoreWeb.Models;

namespace BookstoreWebNUnitTest.Repository.IRepository;
    public interface ITestCoverTypeRepo : ITestRepository<CoverType> 
    {
        void Update(CoverType obj);
        IEnumerable<CoverType> GetAll();
    }
