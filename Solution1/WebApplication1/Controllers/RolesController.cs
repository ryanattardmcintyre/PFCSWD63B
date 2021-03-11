using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public RolesController(UserManager<IdentityUser> userManager)
        { _userManager = userManager; }

        [Authorize]
        public async Task<IActionResult> AllocateRole(string role)
        {
            if (role == "User" || role=="Driver")
            {

                var username = HttpContext.User.Identity.Name;

                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                    await _userManager.AddToRoleAsync(user, role);
            }

            return View();
        }
    }
}
