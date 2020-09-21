using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sinder.Helpers;
using Sinder.Models;

namespace Sinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Validate, else redirect to /login
            var cookies = Request.Cookies["token"];
            if(cookies == null)
                return Redirect("/login");
            InfoHelper.IsLoggedIn = true;

            List<UserModel> users = new List<UserModel>();
            users = await Dataprovider.Instance.ReadAllUsers();
            foreach (var user in users)
            {
                List<ImageModel> images = await Dataprovider.Instance.GetUserImagesByUserID(user.ID);
                user.Images = images;
            }
            return View(users);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
