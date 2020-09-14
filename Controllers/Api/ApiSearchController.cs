using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers.Api
{
    [Route("api/search")]
    [ApiController]
    public class ApiSearchController : ControllerBase
    {
        // GET: api/<ApiSearchController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            var queryString = HttpContext.Request.Query["q"];
            var users = Dataprovider.Instance.SearchUser(queryString);

            return new JsonResult(new ResponseModel { Status = users.ToString(), Message = "welcome" }, new JsonSerializerOptions
            {
                WriteIndented = true,
            });



            //return new JsonResult(name = users[0].Name, new JsonSerializerOptions
            //{
            //    WriteIndented = true,
            //});

            //return new string[] { users[0].Name.ToString() };
        }
       
        

        // GET api/<ApiSearchController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiSearchController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ApiSearchController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiSearchController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
