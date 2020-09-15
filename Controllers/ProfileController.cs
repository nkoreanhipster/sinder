using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;

namespace Sinder.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            var cookies = Request.Cookies["token"];
            if (cookies == null)
                return Redirect("/login");
            SecurityHelper.GetLoggedInUser(cookies);
            return View();
        }
    }
}