using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sinder.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            // Validate, else redirect to /login
            //var cookies = Request.Cookies["token"];
            //if (cookies == null)
            //    return Redirect("/login");
            return View();
        }
    }
}