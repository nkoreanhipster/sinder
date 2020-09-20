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


        // Adding a new image to existing user
        [HttpPost("{userId}")]
        public async Task<IActionResult> PostAndLinkToImageId(IFormFile file, int userId)
        {
            if (file == null)
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

            // Upload image URL to sinder database
            await Dataprovider.Instance.AddUserImage(userId, imageModel.Url);
          

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

        /// Delete existing image by ID
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            await Dataprovider.Instance.DeleteImageById(imageId);

            ResponseModel responseModel = new ResponseModel()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Din bild har tagits bort",
                Token = null,
                IsSuccess = true
            };
            return new JsonResult(responseModel, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }
    }
}
