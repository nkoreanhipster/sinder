using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Token { get; set; } = null;
    }
}
