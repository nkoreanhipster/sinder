using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sinder.Controllers.Api;
using Sinder.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder.Controllers
{
    public class SinnersController : Controller
    {
        // GET: SinnersController
        public async Task<IActionResult> Index()
        {
            var cookies = Request.Cookies["token"];
            if (cookies == null || cookies == "null")
                return Redirect("/login");
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel user = await Dataprovider.Instance.ReadUserByEmail(email);

            List<RelationshipDto> model = await Dataprovider.Instance.ReadUserRelationships(user.ID);
            model.ForEach(m => m.CurrentUserID = user.ID);
            return View(model);
        }
    }
}
