﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Sinder.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers.Api
{
    [Route("api/profile")]
    [ApiController]
    public class ApiProfileController : ControllerBase
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,[FromBody] UpdateUserDto user)
        {
            UserModel updateUser = new UserModel();
            updateUser = await Dataprovider.Instance.ReadUserById(id);
            var parts = SecurityHelper.GetPassword(user.Password);
            updateUser.HashedPassword = parts.passwordhash;
            updateUser.Salt = parts.salt;
            updateUser.Firstname = user.FirstName;
            updateUser.Surname = user.Surname;
            await Dataprovider.Instance.UpdateUser(updateUser);

            return new JsonResult(new ResponseModel { Status = "Success", Message = "Användaren är nu uppdaterad" }, new JsonSerializerOptions
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