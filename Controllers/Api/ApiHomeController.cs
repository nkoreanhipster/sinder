using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers.Api
{
    [Route("api/home")]
    [ApiController]
    public class ApiHomeController : ControllerBase
    {
        // GET: api/<ApiHomeController>
        [HttpGet]
        public async Task<IEnumerable<UserModel>> Get()
        {
            List<UserModel> users = new List<UserModel>();
            users = await Dataprovider.Instance.ReadAllUsers();
            return users;
        }

        // GET api/<ApiHomeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiHomeController>
        [HttpPost]
        public void Post([FromBody] UserModel user)
        {
            var ID = user.ID;
        }

        // PUT api/<ApiHomeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] string AcceptOrDecline)
        {
            if (AcceptOrDecline == "Decline")
            {
                return await DeclineUser(id);
            }
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

            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Success", Message = "Vänförfrågan skickad!" }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        private async Task<IActionResult> DeclineUser(int id)
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

            UserModel ProtagonistUser = await Dataprovider.Instance.ReadUserById(id);
            if (await Dataprovider.Instance.CheckIfRelationshipExists(loggedinUser.ID, id))
            {
                await Dataprovider.Instance.MatchRelationship(loggedinUser.ID, id, Relationship.Declined);
            }
            else
            {
                await Dataprovider.Instance.AddUserRelationship(loggedinUser.ID, id, Relationship.Declined);
            }
            
            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Success", Message = $"Du har nu nekat {ProtagonistUser.Firstname}" }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }


        // DELETE api/<ApiHomeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
