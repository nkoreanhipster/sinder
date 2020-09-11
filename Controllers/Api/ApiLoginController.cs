using System;
using System.Collections.Generic;
using System.Linq;
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
            
            if (Users.Count < 1)
            {
                return Unauthorized("Incorrect email");
            }
            if (!PasswordHelper.VerifyPasswordHash(userLogin.Password, Users[0].HashedPassword, Users[0].Salt))
            {
                return Unauthorized("Incorrect password");
            }

            return Accepted("Welcome");

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
