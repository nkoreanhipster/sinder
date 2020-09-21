using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            string queryString = HttpContext.Request.Query["q"];
            if (queryString == null)
                queryString = "";
            List<UserModel> users = new List<UserModel>();
            users.Clear();
            users = await Dataprovider.Instance.SearchUsersBy(queryString);

            //var images = await Dataprovider.Instance.GetUserImagesWhichIsInList(users);
            users.ForEach(async item => item.Images = await Dataprovider.Instance.GetUserImagesByUserID(item.ID));
            users.ForEach(async item => item.Interests = await Dataprovider.Instance.GetUserInterests(item.ID));

            return new JsonResult(users)
            {
                StatusCode = (int)HttpStatusCode.OK
            };

        }



        // GET api/<ApiSearchController>/5
        [HttpGet("{value}")]
        public async Task<List<InterestModel>> Get(string value)
        {
            return await Dataprovider.Instance.GetMatchingInterest(value);
        }
    }
}
