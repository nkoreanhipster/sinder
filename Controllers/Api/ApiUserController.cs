using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiUserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
