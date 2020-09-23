using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sinder.Controllers.Api
{
    [Route("api/message")]
    [ApiController]
    public class ApiMessageController : ControllerBase
    {
        // http://127.0.0.1:5000/api/message/new/30
        [HttpGet("new/{userId}")]
        public async Task<IActionResult> Check(int userId)
        {
            List<MessageModel> newMessages = new List<MessageModel>();
            newMessages = await Dataprovider.Instance.GetAllMessagesByStatus(userId, 0);
            
            return new JsonResult(newMessages);
        }

        //// 127.0.0.1:5000/api/message/read/30
        //[HttpGet("read/{messageId}")]
        //public async Task<IActionResult> SetMessageToRead(int messageId)
        //{
        //    List<MessageModel> newMessages = new List<MessageModel>();
        //    newMessages = await Dataprovider.Instance.GetMe(userId, 0);

        //    return new JsonResult(true);
        //}

        // 127.0.0.1:5000/api/message/read/30/all
        [HttpGet("read/{userId}/all")]
        public async Task<IActionResult> SetAllMessagesToRead(int userId)
        {
            List<RelationshipDto> rrrr = new List<RelationshipDto>();
            rrrr = await Dataprovider.Instance.ReadUserRelationships(userId);

            return new JsonResult(rrrr);
        }
    }
}
