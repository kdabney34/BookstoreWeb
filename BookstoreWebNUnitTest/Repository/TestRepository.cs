using BookstoreWeb.DataAccess;
using BookstoreWeb.DataAccess.Repository.IRepository;
using BookstoreWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreWeb.DataAccess.Repository
{
    public class TestRepository<T> : ITestRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public TestRepository(ApplicationDbContext db)
        {
            _db = db;
            //_db.Products.Include(u => u.Category).Include(u=>u.CoverType);
            this.dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        ////includeProp - "Category,CoverType"
        //public IEnumerable<T> GetAllProduct(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        //{
        //    IQueryable<T> query = dbSet;//since TestRepository demands a class 'T' be passed to it, it becomes understood that saying 'dbSet'
        //    if (filter != null)          // is == saying a DBSet of whatever class 'T' you passed to it in that particular call
        //    {
        //        query = query.Where(filter); //this applies the u=> u.Id==id function
        //    }
        //    if (includeProperties != null)
        //    {
        //        foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            query = query.Include(includeProp);
        //        }
        //    }
        //    return query.ToList();
        //}

        public IEnumerable<T> GetAllProduct(string includeProperties)
        {
            IQueryable<T> query = dbSet;
            if (includeProperties!=null)
            {
                foreach(var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(includeProp);
                }
            }
            return query.ToList();
        }


        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
        {


            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }
            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
