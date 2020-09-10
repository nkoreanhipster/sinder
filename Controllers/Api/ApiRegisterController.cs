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
    [Route("api/[controller]")]
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
            var UserPasswords = PasswordHelper.GetPassword(user.Password);
            await AddNewUser(user, UserPasswords.passwordhash, UserPasswords.salt);

            return new JsonResult(new ResponseModel { Status = "Success", Message = "Användaren är nu registrerad" }, new JsonSerializerOptions
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
