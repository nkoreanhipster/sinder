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

            List<RelationshipDto> model = await Dataprovider.Instance.ReadUserRelationships(user.ID);
            model.ForEach(m => m.CurrentUserID = user.ID);

            // EUGHGEGUEGHEGUGEH långt ifrån bra löst. 
            // Trodde jag skulle vara så jävla cool med 1 model och 1 sql-query
            foreach (var m in model)
            {
                // Swap places to be less annoying in the view
                if(m.CurrentUserID == (int)m.AntagonistID)
                {
                    UserModel actualAntonist = await Dataprovider.Instance.ReadUserById(m.ProtagonistID);
                    Relationship a = m.Status1;
                    m.Status1 = m.Status2;
                    m.Status2 = a;
                    m.ProtagonistFirstName = user.Firstname;
                    m.ProtagonistID = user.ID;
                    m.AntagonistID = actualAntonist.ID;
                    m.AntagonistFirstName = actualAntonist.Firstname;
                    m.Images = await Dataprovider.Instance.GetUserImagesByUserID(m.AntagonistID);
                }
            }

            return View(model);
        }
    }
}
