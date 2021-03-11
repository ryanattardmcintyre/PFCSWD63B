using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Domain;

namespace WebApplication1.DataAccess.Repositories
{
    public class FirestoreBlogRepository : IBlogsRepository
    {
        IConfiguration _config;
        FirestoreDb db;
        public FirestoreBlogRepository(IConfiguration config)
        {   _config = config;
            string projectId = _config.GetSection("ProjectId").Value;
            db = FirestoreDb.Create(projectId);
        }

        public void DeleteBlog(Guid id)
        {
            throw new NotImplementedException();
        }

        public Blog GetBlog(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Blog> GetBlogs()
        {
            throw new NotImplementedException();
        }

        public Guid InsertBlog(Blog b)
        {
            var id = Guid.NewGuid();
            DocumentReference docRef = db.Collection("blogs").Document();
            Dictionary<string, object> blog = new Dictionary<string, object>
            {
                { "Id", id },
                { "Url", b.Url },
                { "Title", b.Title }
            };
            docRef.SetAsync(blog).Wait();
            return id;
        }
    }
}
