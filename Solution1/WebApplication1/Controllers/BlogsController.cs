using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.DataAccess.Repositories;
using WebApplication1.Domain;

namespace WebApplication1.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogsRepository _blogsRepo;
        private readonly IConfiguration _config;
        private readonly IPubSubRepository _pubSubRepo;
        public BlogsController (IBlogsRepository blogsRepo, IConfiguration config, IPubSubRepository pubSubRepo)
        {
            _config = config;
            _blogsRepo = blogsRepo;
            _pubSubRepo = pubSubRepo;
        }
        
        public IActionResult Index()
        {
            return View(_blogsRepo.GetBlogs());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(IFormFile logo, Blog b )
        {
            try
            {
                string bucketName = _config.GetSection("AppSettings").GetSection("PicturesBucket").Value;

                //upload logo in cloud storage
                string filename = Guid.NewGuid() + Path.GetExtension(logo.FileName);

                b.Url = $"https://storage.googleapis.com/{bucketName}/{filename}";
          


                //uploading the physical in cloud storage bucket

                var storage = StorageClient.Create();
                using (var fileToUpload = logo.OpenReadStream())
                {
                 
                    storage.UploadObject(bucketName, filename, null, fileToUpload);
                }

                //insert info in db
                _blogsRepo.InsertBlog(b);


                //sending email as soon as blog is saved in db (instead of a notification)
                _pubSubRepo.PublishEmail(HttpContext.User.Identity.Name, b);

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["error"] = "Failed to upload";
                return View();
            }
        }

        public IActionResult Delete(Guid id)
        {
            try
            {
                string bucketName = _config.GetSection("AppSettings").GetSection("PicturesBucket").Value;
                var storage = StorageClient.Create();

                var url = System.IO.Path.GetFileName(_blogsRepo.GetBlog(id).Url);

                storage.DeleteObject(bucketName, url);
                _blogsRepo.DeleteBlog(id);
            }
            catch(Exception ex)
            { }
            return RedirectToAction("Index");
        }
    }
}
