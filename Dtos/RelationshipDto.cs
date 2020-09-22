using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    /// <summary>
    /// Used to transport your relation to user for list view
    /// </summary>
    public class RelationshipDto
    {
        public int CurrentUserID { get; set; } = -1;
        public int RelationShipID { get; set; }
        public int ProtagonistID { get; set; }
        public string ProtagonistFirstName { get; set; }
        public string AntagonistFirstName { get; set; }
        public int AntagonistID { get; set; }
        public Relationship Status1 { get; set; }
        public Relationship Status2 { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
    }
}
