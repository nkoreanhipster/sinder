using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers.Api
{
    [Route("api/image")]
    [ApiController]
    public class ApiImageController : ControllerBase
    {
        // GET: api/<ImageController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<ImageModel> images = new List<ImageModel>();
            images = await Dataprovider.Instance.GetAllUserImages();
            return new JsonResult(images, new JsonSerializerOptions { WriteIndented = true });
        }

        // GET api/image/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            ImageModel image = new ImageModel();
            List<ImageModel> images = new List<ImageModel>();
            images = await Dataprovider.Instance.GetUserImagesByImageId(id);
            if (images.Count < 1)
                return "no image by that that id";
            return images.First().Url;
        }

        //// POST api/<ImageController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{

        //}

        //// PUT api/<ImageController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ImageController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await Dataprovider.Instance.DeleteImageById(id);
        }
    }
}
