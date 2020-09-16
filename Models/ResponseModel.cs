using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public class ResponseModel
    {
        public int StatusCode { get; set; } = 0;
        public string Status { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Token { get; set; } = null;
        public List<dynamic> Data { get; set; } = new List<dynamic>();
    }
}
