using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BookstoreWeb.Models;

namespace BookstoreWebNUnitTest.Repository.IRepository;
    public interface ITestProductRepo : ITestRepository<Product> 
    {
        void Update(Product obj);
        IEnumerable<Product> GetAll();
    }
