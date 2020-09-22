using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers.Api
{
    [Route("api/user")]
    [ApiController]
    public class ApiUserController : ControllerBase
    {
        // GET: api/user
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/user/[id]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/user/resetpassword
        [HttpPost("{resetpassword}")]
        public async Task<IActionResult> Post(string passwordreset, [FromBody] UserLoginDto user)
        {
            var userPassword = SecurityHelper.GetPassword(user.Password);
            UserModel userToUpdate = new UserModel() { Email = user.Email, HashedPassword = userPassword.passwordhash, Salt = userPassword.salt };
            await Dataprovider.Instance.UpdateUserPassword(userToUpdate);
            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.OK, Status = "Success", Message = "Ändrat Lösenord" }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        // GET api/[id]/message/[id]
        // Get converstion between User[id] and User[id]
        [HttpGet("{subjectID}/message/{targetID}")]
        public async Task<IActionResult> GetMessageHistory(int subjectID, int targetID)
        {
            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.NotImplemented, Status = "Meh", Message = "Not yet implemented" }, new JsonSerializerOptions { WriteIndented = true });
        }

        // PUT api/[id]/message/[id]
        // Love request throug the match function
        [HttpPut("{subjectID}/message/{targetID}")]
        public async Task<IActionResult> SendMessage(int subjectID, int targetID)
        {
            if (await Dataprovider.Instance.CheckIfRelationshipExists(subjectID, targetID))
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "Ni behöver vara vänner först" }, new JsonSerializerOptions { WriteIndented = true });



            // Not yet implemented
            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.NotImplemented, Status = "Meh", Message = "Not yet implemented" }, new JsonSerializerOptions { WriteIndented = true });

            //return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.OK, Status = "Success", Message = "Meddelande skickat" }, new JsonSerializerOptions { WriteIndented = true });
        }

        // PUT api/[id]/loves/[id]
        // Love request throug the match function
        [HttpGet("{subjectID}/loves/{targetID}")]
        public async Task<IActionResult> LoveShip(int subjectID, int targetID)
        {
            if (await Dataprovider.Instance.CheckIfRelationshipExists(subjectID, targetID))
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "Du har redan skickat förfrågan" }, new JsonSerializerOptions { WriteIndented = true });
            await Dataprovider.Instance.AddUserRelationship(subjectID, targetID, Relationship.Friend);

            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.OK, Status = "Success", Message = "Vänförfrågan skickad!" }, new JsonSerializerOptions { WriteIndented = true });
        }

        // PUT api/user/[id]
        // Friend request
        // id in parameter is the target users Id
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            var cookies = Request.Cookies["token"];
            string email = SecurityHelper.GetLoggedInUser(cookies);
            UserModel loggedinUser = await Dataprovider.Instance.ReadUserByEmail(email);
            if (loggedinUser.ID == id)
            {
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "Du kan inte skicka en vänförfrågan till dig själv!" }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            }

            bool hasRelationship = await Dataprovider.Instance.CheckIfRelationshipExists(loggedinUser.ID, id);
            if (hasRelationship == true)
            {
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "En vänförfrågan har redan skickats. Kolla dina vänförfrågningar!" }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            }

            await Dataprovider.Instance.AddUserRelationship(loggedinUser.ID, id, Relationship.Friend);

            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.OK, Status = "Success", Message = "Vänförfrågan skickad!" }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        // DELETE api/user/[id]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        /// <summary>
        /// Test check matching
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        //[HttpGet("{id1}/{id2}")]
        //public async Task<IEnumerable<double>> GetMatchPercentage(int id1, int id2)
        //{
        //    var userA = await Dataprovider.Instance.ReadUserById(id1);
        //    var userB = await Dataprovider.Instance.ReadUserById(id2);
        //    if(userA == null || userB == null)
        //        return new List<double>() { 9999.0 };

        //    userA.Interests = await Dataprovider.Instance.GetUserInterests(id1);
        //    userB.Interests = await Dataprovider.Instance.GetUserInterests(id2);

        //    MatchAlgorithm m = new MatchAlgorithm();
        //    m.SetSubject(userA);
        //    m.AddTarget(userB);

        //    return m.GetInterestMatchPercentages();
        //}
    }
}
