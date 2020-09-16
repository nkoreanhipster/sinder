using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder.Controllers
{
    public class ProfileController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var cookies = Request.Cookies["token"];
            if (cookies == null || cookies =="null")
                return Redirect("/login");
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel user = await Dataprovider.Instance.ReadUser(email);
            return View(user);
        }
        public async Task<IActionResult> User(int id)
        { 

            UserModel user = await Dataprovider.Instance.ReadUserById(id);
            return View(user);
        }

        public async Task<IActionResult> EditeProfile()
        {
            var cookies = Request.Cookies["token"];
            if (cookies == null || cookies == "null")
                return Redirect("/login");
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel user = await Dataprovider.Instance.ReadUser(email);
            return View(user);
        }
    }
}