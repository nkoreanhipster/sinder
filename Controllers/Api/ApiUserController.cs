using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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


        // PUT api/user/[id]/relationsship
        // Love request throug the match function
        [HttpGet("{subjectID}/relationship")]
        public async Task<IActionResult> ListRelations(int subjectID)
        {
            var x = Dataprovider.Instance.ReadUserRelationships(subjectID);
            return new JsonResult(x);

        }

        // GET api/user/[id]/message/[id]/list
        // Get converstion between User[id] and User[id]
        [HttpGet("{subjectID}/message/{targetID}/list")]
        public async Task<IActionResult> GetMessageHistory(int subjectID, int targetID)
        {
            var hasRelation = await Dataprovider.Instance.CheckIfRelationshipExists(subjectID, targetID);
            if (!hasRelation)
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "Ni behöver vara vänner först" }, new JsonSerializerOptions { WriteIndented = true });

            // Hämta id från relationen mellan users
            int relationshipId = await Dataprovider.Instance.ReadRealtionShipId(subjectID, targetID);

            // Validera, i mySql börjar alltid alla id från 1
            if (relationshipId < 0)
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.BadRequest, Status = "Fail", Message = "Kunde ej skicka" }, new JsonSerializerOptions { WriteIndented = true });

            List<MessageModel> messages = await Dataprovider.Instance.ReadAllMessagesBetweenTwoUsers(relationshipId);

            return new JsonResult(messages);
        }

        // PUT api/user/[id]/message/[id]
        // Love request throug the match function
        [HttpPut("{subjectID}/message/{targetID}")]
        public async Task<IActionResult> SendMessage(int subjectID, int targetID, [FromBody] MessageDto message)
        {
            try
            {
                var hasRelation = await Dataprovider.Instance.CheckIfRelationshipExists(subjectID, targetID);
                if (!hasRelation)
                    return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "Ni behöver vara vänner först" }, new JsonSerializerOptions { WriteIndented = true });

                // Hämta id från relationen mellan users
                int relationshipId = await Dataprovider.Instance.ReadRealtionShipId(subjectID, targetID);

                // Validera, i mySql börjar alltid alla id från 1
                if (relationshipId < 0)
                    return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.BadRequest, Status = "Fail", Message = "Kunde ej skicka" }, new JsonSerializerOptions { WriteIndented = true });

                // Put message data into database
                await Dataprovider.Instance.SendMessageFromTo(relationshipId, subjectID, message.Message);
            }
            catch
            {
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.InternalServerError, Status = "Fail", Message = "Något gick fel" }, new JsonSerializerOptions { WriteIndented = true });
            }

            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.OK, Status = "Success", Message = message.Message }, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Accepting someone's friend or love request, matching the type
        /// ROUTE => PUT api/user/[id]/accept/[id]/[relationshiptype]
        /// </summary>
        [HttpGet("{subjectID}/accept/{targetID}/{relationShipType}")]
        public async Task<IActionResult> Accept(int subjectID, int targetID, int relationShipType)
        {
            // Break if there is no current relationship
            if (await Dataprovider.Instance.CheckIfRelationshipExists(subjectID, targetID) == false)
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "Ni har inga stående förfrågningar mellan varandra" }, new JsonSerializerOptions { WriteIndented = true });

            Relationship r = Relationship.Declined;

            switch (relationShipType)
            {
                case 1:
                    r = Relationship.Friend;
                    break;
                case 2:
                    r = Relationship.Love;
                    break;
            }

            // Break, we are only accepting type 1 and 2
            if (r != Relationship.Friend && r != Relationship.Love)
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "Felaktiga värden" }, new JsonSerializerOptions { WriteIndented = true });

            await Dataprovider.Instance.MatchRelationship(subjectID, targetID, r);

            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.OK, Status = "Success", Message = "Ni är nu vänner och/eller sinners" }, new JsonSerializerOptions { WriteIndented = true });
        }

        // PUT api/user/[id]/loves/[id]
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
