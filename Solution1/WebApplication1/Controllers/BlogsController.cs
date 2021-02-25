using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.DataAccess.Repositories;

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


        
    }
}
