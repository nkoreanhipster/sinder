using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
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
        public static string GetSecretApiKey() => ConfigurationManager.AppSettings["secret"];
        public static bool IsNumber(string text)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(text);
        }
    }
}
