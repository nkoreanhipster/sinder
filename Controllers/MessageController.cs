using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace Sinder.Controllers
{
    public class MessageController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet("message/{relationshipId}")]
        public async Task<IActionResult> ChatBox(int relationshipId)
        {
            // Validate, else redirect to /login
            var cookies = Request.Cookies["token"];
            if (cookies == null)
                return Redirect("/login");
            InfoHelper.IsLoggedIn = true;

            // Get current active user
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel currentUser = await Dataprovider.Instance.ReadUserByEmail(email);


            // Check if the relationshipID exists
            if (!await Dataprovider.Instance.CheckIfRelationshipExists(relationshipId))
                return Unauthorized("403");

            RelationShipModel relation = await Dataprovider.Instance.ReadRelationShipById(relationshipId);

            // Validate same level of accepted relation
            if (!await Dataprovider.Instance.CheckIfSameRelationShip(relation.UserID1, relation.UserID2))
                return Unauthorized("403");

            if(currentUser.ID != relation.UserID1 && currentUser.ID != relation.UserID2)
                return Unauthorized("403");

            UserModel antagonist = await Dataprovider.Instance.ReadUserById(currentUser.ID != relation.UserID1 ? relation.UserID1 : relation.UserID2);
            antagonist.Images = await Dataprovider.Instance.GetUserImagesByUserID(antagonist.ID);
            currentUser.Images = await Dataprovider.Instance.GetUserImagesByUserID(currentUser.ID);
            List<MessageModel> messages = new List<MessageModel>();
            var _ = await Dataprovider.Instance.ReadAllMessagesBetweenTwoUsers(relationshipId);
            foreach (MessageModel m in _)
            {
                messages.Add(m);
            }

            dynamic model = new System.Dynamic.ExpandoObject();

            model.Me = currentUser;
            model.NotMe = antagonist;
            model.Messages = messages;

            //(UserModel, UserModel, List<MessageModel>) tuple = (currentUser, antagonist, messages);
            return View(model);
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet("message/{UserID1}/{UserID2}")]
        public async Task<IActionResult> ChatBox(int UserID1, int UserID2)
        {
            // Validate, else redirect to /login
            var cookies = Request.Cookies["token"];
            if (cookies == null)
                return Redirect("/login");
            InfoHelper.IsLoggedIn = true;

            // Get current active user
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel currentUser = await Dataprovider.Instance.ReadUserByEmail(email);

            int relationshipId = await Dataprovider.Instance.ReadRealtionShipId(UserID1,UserID2);
            // Check if the relationshipID exists
            if (!await Dataprovider.Instance.CheckIfRelationshipExists(relationshipId))
                return Unauthorized("403");

            RelationShipModel relation = await Dataprovider.Instance.ReadRelationShipById(relationshipId);

            // Validate same level of accepted relation
            if (!await Dataprovider.Instance.CheckIfSameRelationShip(relation.UserID1, relation.UserID2))
                return Unauthorized("403");

            if (currentUser.ID != relation.UserID1 && currentUser.ID != relation.UserID2)
                return Unauthorized("403");

            UserModel antagonist = await Dataprovider.Instance.ReadUserById(currentUser.ID != relation.UserID1 ? relation.UserID1 : relation.UserID2);
            antagonist.Images = await Dataprovider.Instance.GetUserImagesByUserID(antagonist.ID);
            currentUser.Images = await Dataprovider.Instance.GetUserImagesByUserID(currentUser.ID);
            List<MessageModel> messages = new List<MessageModel>();
            var _ = await Dataprovider.Instance.ReadAllMessagesBetweenTwoUsers(relationshipId);
            foreach (MessageModel m in _)
            {
                messages.Add(m);
            }

            dynamic model = new System.Dynamic.ExpandoObject();

            model.Me = currentUser;
            model.NotMe = antagonist;
            model.Messages = messages;

            //(UserModel, UserModel, List<MessageModel>) tuple = (currentUser, antagonist, messages);

            return View(model);
        }
    }
}
