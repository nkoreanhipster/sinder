using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder.Models
{
    public enum Relationships
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Blocked = -1
    }
}
