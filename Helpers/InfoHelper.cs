using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public static class InfoHelper
    {
        public static bool IsLoggedIn { get; set; } = false;
        //public static int UserLoggedInID { get; set; }
        private static UserModel loggedInUser = new UserModel();
        public static UserModel LoggedInUser
        {
            get => IsLoggedIn ? loggedInUser : new UserModel();
            set => InfoHelper.loggedInUser = InfoHelper.IsLoggedIn ? value : new UserModel();
        }
    }
}
