using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sinder.Controllers.Api;
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

            List<UserMatchDto> recieved = await Dataprovider.Instance.ReadRecievedRequests(user.ID);
            List<UserMatchDto> requested = await Dataprovider.Instance.ReadRequests(user.ID);
            List<UserMatchDto> matched = await Dataprovider.Instance.ReadMatches(user.ID);
            matched.RemoveAll(x => x.ID == user.ID);
            (UserModel,List<UserMatchDto>, List<UserMatchDto>, List<UserMatchDto>) tuple = (user,recieved, requested, matched);
            return View(tuple);
        }
    }
}
