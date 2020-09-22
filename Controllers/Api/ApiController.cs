using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        private readonly IActionDescriptorCollectionProvider _provider;

        public ApiController(IActionDescriptorCollectionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Get all active API routes
        /// </summary>
        [HttpGet]
        public IActionResult GetRoutes()
        {
            var routes = _provider.ActionDescriptors.Items
                .Select(ad => new
                {
                    Action = ad.RouteValues["action"],
                    Controller = ad.RouteValues["controller"],
                }).Distinct().ToList().OrderBy(obj => obj.Controller);

            return Ok(routes);
        }
    }
}
