using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public enum Relationship
    {
        Blocked = -2,
        Declined = -1,
        Pending = 0,
        Friend = 1,
        Love = 2,
        Super = 3
    }
}
