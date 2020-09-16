using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sinder
{
    public class UserRegistrationDto
    {
        [Required]
        [EmailAddress(ErrorMessage ="Felaktig syntax på email")]
        public string Email { get; set; }

        [Required]
        [StringLength(20,MinimumLength = 6, ErrorMessage = "Du måste fylla fylla i ett lösenord med minst 6 tecken")]
        public string Password { get; set; }

        [Required]
        [StringLength(20, MinimumLength =2, ErrorMessage = "Du måste fylla i ett förnamn, endast bokstäver")]
        [RegularExpression(@"[a-zA-ZåäöÅÄÖ]+", ErrorMessage = "Du måste fylla i ett förnamn, endast bokstäver")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ett efternamn")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Du måste fylla i ett efternamn, endast bokstäver")]
        [RegularExpression(@"[a-zA-ZåäöÅÄÖ]+", ErrorMessage = "Du måste fylla i ett efternamn, endast bokstäver")]
        public string Surname { get; set; }

        public string Location { get; set; }

        [Required]
        public DateTime Age { get; set; }

        [Required(ErrorMessage ="Felaktig inmatning vid kön")]
        [RegularExpression(@"[Mm]an|[Kk]vinna|[Aa]nnat", ErrorMessage = "Felaktig inmatning vid kön")]
        public string Gender { get; set; }

        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
    }
}
