using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ApiHomeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiHomeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
