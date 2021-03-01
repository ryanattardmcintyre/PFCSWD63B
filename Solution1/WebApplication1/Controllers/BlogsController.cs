using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.DataAccess.Repositories;
using WebApplication1.Domain;

namespace WebApplication1.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogsRepository _blogsRepo;
        public BlogsController (IBlogsRepository blogsRepo)
        {
            _blogsRepo = blogsRepo;
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

            string bucketName = "";
            
            //upload logo in cloud storage
            string filename = Guid.NewGuid() + Path.GetExtension(logo.FileName);




            b.Url = $"https://storage.googleapis.com/{bucketName}/{filename}";

            //insert info in db
            _blogsRepo.InsertBlog(b);


            return View();
        }

        
    }
}
