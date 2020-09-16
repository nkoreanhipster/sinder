using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers
{
    [Route("api/register")]
    [ApiController]
    public class ApiRegisterController : ControllerBase
    {
        // GET: api/<UserRegisterController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserRegisterController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserRegisterController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRegistrationDto user)
        {
            List<UserModel> emails = await Dataprovider.Instance.ReadUsersByEmail(user.Email);
            if (emails.Count > 0)
            {
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.Unauthorized, Status = "Failed attempt", Message = "Emailen är redan registrerad" }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            }
            var UserPasswords = SecurityHelper.GetPassword(user.Password);
            await AddNewUser(user, UserPasswords.passwordhash, UserPasswords.salt);

            // IF EXIST, DONT
            return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.OK, Status = "Success", Message = "Användaren är nu registrerad" }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        // PUT api/<UserRegisterController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserRegisterController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private async Task AddNewUser(UserRegistrationDto user, byte[] passwordHash, byte[] salt)
        {
            UserModel newUser = new UserModel();
            newUser.Email = user.Email;
            newUser.Firstname = user.FirstName;
            newUser.Surname = user.Surname;
            newUser.Age = user.Age;
            newUser.Gender = user.Gender;
            newUser.HashedPassword = passwordHash;
            newUser.Salt = salt;
            newUser.Location = user.Location;

            await Dataprovider.Instance.RegisterNewUser(newUser);
        }
    }
}
