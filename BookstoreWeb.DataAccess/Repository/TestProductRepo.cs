//using BookstoreWeb.DataAccess;
//using BookstoreWeb.DataAccess.Repository.IRepository;
//using BookstoreWeb.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BookstoreWeb.DataAccess.Repository
//{
//    public class TestProductRepo : TestRepository<Product>, ITestProductRepo
//    {
//        private ApplicationDbContext _db;

//        public TestProductRepo(ApplicationDbContext db) : base(db)
//        {
//            _db = db;
//        }


//        public IEnumerable<T> GetAllProduct(string includeProperties)
//        {
//            IQueryable<T> query = dbSet;
//            if (includeProperties != null)
//            {
//                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
//                {
//                    query = query.Include(includeProp);
//                }
//            }
//            return query.ToList();
//        }


//        public void Update(Product obj)
//        {
//            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
//            if (objFromDb != null)
//            {
//                objFromDb.Title = obj.Title;
//                objFromDb.ISBN = obj.ISBN;
//                objFromDb.Price = obj.Price;
//                objFromDb.Price50 = obj.Price50;
//                objFromDb.ListPrice = obj.ListPrice;
//                objFromDb.Price100 = obj.Price100;
//                objFromDb.Description = obj.Description;
//                objFromDb.CategoryId = obj.CategoryId;
//                objFromDb.Author = obj.Author;
//                objFromDb.CoverTypeId = obj.CoverTypeId;
//                if (obj.ImageUrl != null)
//                {
//                    objFromDb.ImageUrl = obj.ImageUrl;
//                }
//            }
//        }
//    }
//}