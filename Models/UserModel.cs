using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public class UserModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Location { get; set; }
        public int Age { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] Salt { get; set; }
        public string Gender { get; set; }
    }
}
