using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sinder.Models;
using System.Linq;

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
            users = await Dataprovider.Instance.ReadAllUsers();
            int indexOfCurrentUser = 0;
            foreach (var user in users)
            {
                if (user.ID == currentUser.ID)
                {
                    indexOfCurrentUser = users.IndexOf(user);
                }
                user.Images = await Dataprovider.Instance.GetUserImagesByUserID(user.ID);
                user.Interests = await Dataprovider.Instance.GetUserInterests(user.ID);
            }
            
            //Removes the logged in user from the list
            users.RemoveAt(indexOfCurrentUser);

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

            return View(usersToBeMatched);
        }

        //[HttpGet("/test")]
        //public async Task<IActionResult> IndexTest()
        //{
        //    // Validate, else redirect to /login
        //    var cookies = Request.Cookies["token"];
        //    if (cookies == null)
        //        return Redirect("/login");
        //    InfoHelper.IsLoggedIn = true;


        //    // Get current active user
        //    string email = SecurityHelper.GetLoggedInUser(cookies);
        //    UserModel currentUser = await Dataprovider.Instance.ReadUserByEmail(email);
        //    currentUser.Images = await Dataprovider.Instance.GetUserImagesByUserID(currentUser.ID);
        //    currentUser.Interests = await Dataprovider.Instance.GetUserInterests(currentUser.ID);

        //    // Antoganists
        //    List<MatchUserDtocs> usersToBeMatched = new List<MatchUserDtocs>();
        //    List<UserModel> users = new List<UserModel>();
        //    users = await Dataprovider.Instance.ReadAllUsers();
        //    foreach (var user in users)
        //    {
        //        List<ImageModel> images = await Dataprovider.Instance.GetUserImagesByUserID(user.ID);
        //        user.Images = images;
        //        user.Interests = await Dataprovider.Instance.GetUserInterests(user.ID);
        //    }
            
        //    // Make match thingy
        //    usersToBeMatched = Converters.ConvertUserModelToMatchUserDto(users);
        //    MatchAlgorithm.CalculateMatchPercentage(currentUser, ref usersToBeMatched);
            


        //    return View(usersToBeMatched);
        //}


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
