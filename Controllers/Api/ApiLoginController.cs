using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet.Messages.Authentication;
using Sinder.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class ApiLoginController : ControllerBase
    {
        // GET: api/<ApiLoginController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value5", "value2" };
        }

        // GET api/<ApiLoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiLoginController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserLoginDto userLogin)
        {
            var Users = await Dataprovider.Instance.ReadUsers(userLogin.Email);
            bool isSuccess = true;
            string status = null;
            string message = null;
            string token = null;

            if (Users.Count < 1)
            {
                isSuccess = false;
                status = "Fail";
                message = "Incorrect email";

            }
            if (!SecurityHelper.VerifyPasswordHash(userLogin.Password, Users[0].HashedPassword, Users[0].Salt))
            {
                isSuccess = false;
                status = "Fail";
                message = "Incorrect password";
            }

            // If success, generate JWT token
            if (isSuccess)
            {
                token = SecurityHelper.GenerateToken();
            }

            return new JsonResult(new ResponseModel { Status = status, Message = message, Token = token, IsSuccess = isSuccess }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        // PUT api/<ApiLoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiLoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
