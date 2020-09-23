using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder.Dtos
{
    public class UserRelationshipDto
    {
        public int ID { get; set; }

        public int UserID1 { get; set; }

        public int UserID2 { get; set; }
    }
}
