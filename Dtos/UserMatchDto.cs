using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public class UserMatchDto
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Location { get; set; }
        public DateTime Age { get; set; }
        public Relationship Status1 { get; set; }
        public Relationship Status2 { get; set; }
        public string Gender { get; set; }
        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
        public List<InterestModel> Interests { get; set; } = new List<InterestModel>();
    }
}
