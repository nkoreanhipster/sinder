using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var Users = await Dataprovider.Instance.ReadUsersByEmail(userLogin.Email);
            bool isSuccess = true;
            string status = null;
            string message = null;
            string token = null;

            if (Users.Count < 1)
            {
                isSuccess = false;
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.BadRequest, Status = "Fail", Message = "Incorrect email", Token = token, IsSuccess = isSuccess }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            }
            if (!SecurityHelper.VerifyPasswordHash(userLogin.Password, Users[0].HashedPassword, Users[0].Salt))
            {
                isSuccess = false;
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Fail", Message = "Incorrect password", Token = token, IsSuccess = isSuccess }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            }

            // If success, generate JWT token
            if (isSuccess)
            {
                status = "Success";
                token = SecurityHelper.GenerateToken(userLogin.Email);
            }

            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.OK, Status = status, Message = message, Token = token, IsSuccess = isSuccess }, new JsonSerializerOptions
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
