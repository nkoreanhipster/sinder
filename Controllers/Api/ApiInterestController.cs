using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sinder.Controllers.Api
{
    [Route("api/interest")]
    [ApiController]
    public class ApiInterestController : ControllerBase
    {
        // GET: api/ApiInterest
        [HttpGet]
        public async Task<IEnumerable<InterestModel>> Get()
        {
            string limit = HttpContext.Request.Query["limit"];
            if(!string.IsNullOrEmpty(limit) && !string.IsNullOrWhiteSpace(limit) && Helper.IsNumber(limit))
            {
                int l = Int32.Parse(limit);
                return await Dataprovider.Instance.GetAllInterests(l);
            }
            return await Dataprovider.Instance.GetAllInterests();
        }
    }
}
