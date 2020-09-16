using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Ubiety.Dns.Core.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers.Api
{
    [Route("api/file")]
    [ApiController]
    public class ApiFileController : ControllerBase
    {
        // GET: api/<ApiFileController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ApiFileController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiFileController>
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if(file == null)
            {
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.NotAcceptable, Message = "Fil[erna] kunde ej laddas upp", Token = null, IsSuccess = true }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            }

            // Save locally first
            string uploadedFilePath = FileHelper.Upload(file);

            // Validation files were uploaded
            if (uploadedFilePath == null)
            {
                return new JsonResult(new ResponseModel { StatusCode = (int)HttpStatusCode.InternalServerError, Message = "Fil[erna] gick ej att ladda upp", Token = null, IsSuccess = true }, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
            }

            // Upload to image API-server
            ImageModel imageModel = await WebHelper.UploadProfileImage(uploadedFilePath);
            ResponseModel responseModel = new ResponseModel() 
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Fil[erna] har laddats upp",
                Token = null,
                IsSuccess = true
            };

            responseModel.Data.Add(imageModel);

            return new JsonResult(responseModel, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }

        // PUT api/<ApiFileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiFileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
