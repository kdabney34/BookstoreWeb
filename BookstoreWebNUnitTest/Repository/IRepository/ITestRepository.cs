using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreWebNUnitTest.Repository.IRepository;
    public interface ITestRepository<T> where T : class
	{
		//T - Category
		T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked=true);
		
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        IEnumerable<T> GetAllProduct(string includeProperties);

        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
