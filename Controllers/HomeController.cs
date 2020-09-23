using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sinder.Models;
using System.Linq;
using Sinder.Dtos;

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

            // Get current active user
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel currentUser = await Dataprovider.Instance.ReadUserByEmail(email);
            currentUser.Images = await Dataprovider.Instance.GetUserImagesByUserID(currentUser.ID);
            currentUser.Interests = await Dataprovider.Instance.GetUserInterests(currentUser.ID);

            // Get all active users (todo; limit this somehow, will be ass-slow if database gets larger)
            List<UserModel> users = new List<UserModel>();
            if (currentUser.Gender == "Man")
            {
                users = await Dataprovider.Instance.ReadAllFemale();
            }
            else
            {
                users = await Dataprovider.Instance.ReadAllMale();
            }


            foreach (var user in users)
            {
                user.Images = await Dataprovider.Instance.GetUserImagesByUserID(user.ID);
                user.Interests = await Dataprovider.Instance.GetUserInterests(user.ID);
            }

            var relationships = await Dataprovider.Instance.ReadUserByRelationships(currentUser.ID);
            foreach (var relationship in relationships)
            {

                if (relationship.UserID1 == currentUser.ID)
                {
                    users.RemoveAll(User => User.ID == relationship.UserID2);
                }
                else
                {
                    users.RemoveAll(User => User.ID == relationship.UserID1);
                }
            }

            // Make match thingy
            // Convert usermodels => MatchUserDtocs model
            List<MatchUserDtocs> usersToBeMatched = new List<MatchUserDtocs>();
            usersToBeMatched = Converters.ConvertUserModelToMatchUserDto(users);
            // Match percentage gets added to MatchUserDtocs.ProtagonistMatchPercentage property
            MatchAlgorithm.CalculateMatchPercentage(currentUser, ref usersToBeMatched);

            // Sort by best matches first..?
            usersToBeMatched = usersToBeMatched.OrderBy(x => x.ProtagonistMatchPercentage).ToList();

            // ... Order sorted the wrong way apparently
            usersToBeMatched.Reverse();

            //(List<MatchUserDtocs>, UserModel) homeTupleThingyTooTiredToNameVariablesProperly = (usersToBeMatched, currentUser);

            return View(usersToBeMatched);
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
