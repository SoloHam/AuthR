using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2_0_Client.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() 
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            var token = HttpContext.GetTokenAsync("access_token");

            return View();
        }
    }
}
