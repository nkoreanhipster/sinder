using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    /// <summary>
    /// Static helper class
    /// </summary>
    public static class Helper
    {
        public static string GetConnectionString() => ConfigurationManager.ConnectionStrings["tjackobacco.com"].ConnectionString;
        public static string GetConnectionString(string name) => ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }
}
