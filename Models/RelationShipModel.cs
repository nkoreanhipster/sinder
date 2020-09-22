using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public class RelationShipModel
    {
        public int ID { get; set; }
        public int UserID1 { get; set; }
        public int UserID2 { get; set; }
        public Relationship Status1 { get; set; }
        public Relationship Status2 { get; set; }
    }
}
