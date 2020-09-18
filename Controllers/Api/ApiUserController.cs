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
        // GET: api/<ApiUserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ApiUserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiUserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ApiUserController>/5
        //id in parameter is the target users Id
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

            await Dataprovider.Instance.AddUserRelationship(loggedinUser.ID, id);

            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Success", Message = "Vänförfrågan skickad!" }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        // DELETE api/<ApiUserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
