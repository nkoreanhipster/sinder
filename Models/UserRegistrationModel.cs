using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public class UserRegistrationModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20 ,MinimumLength = 6, ErrorMessage = "Du måste fylla fylla i ett lösenord med minst 6 tecken")]
        public string Password { get; set; }

        //[Required]
        public string FirstName { get; set; }

        //[Required]
        public string Surname { get; set; }

        public string Location { get; set; }

        //[Required]
        public int Age { get; set; }

        //[Required]
        public string Gender { get; set; }
    }
}
