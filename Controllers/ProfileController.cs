using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder.Controllers
{
    public class ProfileController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var cookies = Request.Cookies["token"];
            if (cookies == null || cookies == "null")
                return Redirect("/login");
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel user = await Dataprovider.Instance.ReadUserByEmail(email);
            List<ImageModel> images = await Dataprovider.Instance.GetUserImagesByUserID(user.ID);
            user.Images = images;
            return View(user);
        }
        public async Task<IActionResult> User(int id)
        {
            UserModel user = await Dataprovider.Instance.ReadUserById(id);
            return View(user);
        }

        public async Task<IActionResult> EditProfile()
        {
            var cookies = Request.Cookies["token"];
            if (cookies == null || cookies == "null")
                return Redirect("/login");
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel user = await Dataprovider.Instance.ReadUserByEmail(email);
            List<ImageModel> images = await Dataprovider.Instance.GetAllUserImages();
            user.Images = images;
            return View(user);
        }

        /// <summary>
        /// Just to quickly test-view all user images
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Images()
        {
            List<ImageModel> images = await Dataprovider.Instance.GetAllUserImages();
            UserModel _ = new UserModel() { Images = images };
            return View(_);
        }
    }
}