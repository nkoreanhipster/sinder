using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers
{
    [Route("api")]
    //[Route("api/login")]
    //[Route("api/logout")]
    //[Route("api/search")]
    //[Route("api/profile")]
    //[Route("api/message")]
    //[Route("xxx/[hello]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        // GET: api/<ApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value4", "value5" };
        }

        // GET api/[type]
        [HttpGet("{type}")]
        public string[] Get(string type)
        {
            switch (type)
            {
                case "logout":
                    break;
                case "search":
                    break;
            }
            return new string[] { $"[type]={type}" };
        }

        // POST api/[type]
        [HttpPost("{type}")]
        [Produces("application/json")]
        public async  Task<IActionResult> Post(string type, [FromBody] UserRegistrationModel user )
        {
            bool result = false;
            switch (type)
            {
                case "login":
                    
                    break;
                case "register":
                    var UserPasswords = PasswordHelper.GetPassword(user.Password);
                    await AddNewUser(user ,UserPasswords.passwordhash, UserPasswords.salt);
                    break;
                case "search":
                    break;
                case "message":
                    break;
            }

            return new JsonResult(new ResponseModel { Status = result.ToString(), Message = "Användaren är nu registrerad" }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        private async Task AddNewUser(UserRegistrationModel user ,byte[] passwordHash, byte[] salt)
        {
            UserModel newUser = new UserModel();
            newUser.Email = user.Email;
            newUser.Firstname = user.FirstName;
            newUser.Surname = user.Surname;
            newUser.Age = user.Age;
            newUser.Gender = user.Gender;
            newUser.HashedPassword = passwordHash;
            newUser.Salt = salt;
        }

        //// PUT api/<ApiController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ApiController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
