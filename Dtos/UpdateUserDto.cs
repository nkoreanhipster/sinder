using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public class UpdateUserDto
    {
       
        public string Email { get; set; }

        public string Password { get; set; }

        public string CurrentPassword { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Location { get; set; }

        public DateTime Age { get; set; }

        public string Gender { get; set; }
    }
}
