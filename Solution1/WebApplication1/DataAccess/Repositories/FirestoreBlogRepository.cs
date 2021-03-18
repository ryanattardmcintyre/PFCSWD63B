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
            string projectId = _config.GetSection("AppSettings").GetSection("ProjectId").Value;
            db = FirestoreDb.Create(projectId);
        }

        public void DeleteBlog(Guid id)
        {

            CollectionReference citiesRef = db.Collection("blogs");
            Query query = citiesRef.WhereEqualTo("Id", id.ToString());

            Task<QuerySnapshot> task = query.GetSnapshotAsync();
            task.Wait();
            QuerySnapshot allBlogsQuerySnapshot = task.Result;
            var docRef = allBlogsQuerySnapshot.Documents[0].Reference;
            docRef.DeleteAsync().Wait();
        }

        public Blog GetBlog(Guid id)
        {
            CollectionReference citiesRef = db.Collection("blogs");
            Query query = citiesRef.WhereEqualTo("Id", id.ToString());

            Task<QuerySnapshot> task = query.GetSnapshotAsync();
            task.Wait();
            QuerySnapshot allBlogsQuerySnapshot = task.Result;
            var docRef = allBlogsQuerySnapshot.Documents[0];
            
            Dictionary<string, object> blog = docRef.ToDictionary();
            Blog b= new Blog()
            {
                BlogId = blog.ContainsKey("Id") ? new Guid(blog["Id"].ToString()) : new Guid(),
                Url = blog.ContainsKey("Url") ? blog["Url"].ToString() : "",
                Title = blog.ContainsKey("Title") ? blog["Title"].ToString() : ""
            };
            return b;

        }

        public IQueryable<Blog> GetBlogs()
        {
            Query allBlogsQuery = db.Collection("blogs");
            
            Task<QuerySnapshot> task = allBlogsQuery.GetSnapshotAsync();
            task.Wait();
            QuerySnapshot allBlogsQuerySnapshot =task.Result;

            List<Blog> listOfBlogs = new List<Blog>();

            foreach (DocumentSnapshot documentSnapshot in allBlogsQuerySnapshot.Documents)
            {
              //  Console.WriteLine("Document data for {0} document:", documentSnapshot.Id);
                Dictionary<string, object> blog = documentSnapshot.ToDictionary();
                listOfBlogs.Add(new Blog()
                {
                    BlogId = blog.ContainsKey("Id")? new Guid(blog["Id"].ToString()) : new Guid(),
                    Url = blog.ContainsKey("Url")? blog["Url"].ToString() : "",
                    Title = blog.ContainsKey("Title")? blog["Title"].ToString(): ""
                });  
            }

            return listOfBlogs.AsQueryable();
        }

        public  Guid InsertBlog(Blog b)
        {
            var id = Guid.NewGuid();
            DocumentReference docRef = db.Collection("blogs").Document();
            Dictionary<string, object> blog = new Dictionary<string, object>
            {
                { "Id", id.ToString() },
                { "Url", b.Url },
                { "Title", b.Title }
            };
            docRef.SetAsync(blog).Wait();
            return id;
        }
    }
}
