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
        // GET: api/ApiInterest/QUERY
        [HttpGet("{query}")]
        public async Task<IEnumerable<InterestModel>> Get(string query)
        {
            try
            {
                string limit = HttpContext.Request.Query["limit"];
                if (!string.IsNullOrEmpty(limit) && !string.IsNullOrWhiteSpace(limit) && Helper.IsNumber(limit))
                {
                    int l = Int32.Parse(limit);
                    return await Dataprovider.Instance.GetMatchingInterest(query, l);
                }
                return await Dataprovider.Instance.GetMatchingInterest(query);
            }
            catch
            {
                return new List<InterestModel>();
            }
        }

        /// <summary>
        /// UserID and name of interest
        /// </summary>
        [HttpDelete("{userId}/{name}")]
        public async Task<IEnumerable<string>> Delete(int userId, string name)
        {
            await Dataprovider.Instance.DeleteUserInterest(userId, name);
            return new List<string>() { @"Probably deleted ¯\\\_(ツ)_/¯" };
        }
    }
}
