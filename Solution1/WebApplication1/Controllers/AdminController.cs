using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.Interfaces;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly ICachingService _caching;

        public AdminController(ICachingService caching)
        {
            _caching = caching;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Menu m)
        {
            _caching.UpsertMenu(m);
            return View();
        }
    }
}
