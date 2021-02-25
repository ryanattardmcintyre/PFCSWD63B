using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Domain;

namespace WebApplication1.DataAccess.Repositories
{
    public class BlogsRepository : IBlogsRepository
    {
        private readonly ApplicationDbContext _context;
        public BlogsRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public void DeleteBlog(Guid id)
        {
            _context.Blogs.Remove(GetBlog(id));
            _context.SaveChanges();
        }

        public Blog GetBlog(Guid id)
        {
            return _context.Blogs.SingleOrDefault(x => x.BlogId == id);
        }

        public IQueryable<Blog> GetBlogs()
        {
            return _context.Blogs;
        }

        public Guid InsertBlog(Blog b)
        {
            var id = Guid.NewGuid();
            b.BlogId = id;
            _context.Blogs.Add(b);
            _context.SaveChanges();

            return b.BlogId;
        }
    }
}
