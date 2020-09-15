using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;
using System.Threading.Tasks;

namespace Sinder.Controllers
{
    public class ProfileController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var cookies = Request.Cookies["token"];
            if (cookies == null)
                return Redirect("/login");
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel user = await Dataprovider.Instance.ReadUser(email);
            return View(user);
        }
    }
}