using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public static string GenerateStringFromIds<T>(List<T> items)
        {
            
            StringBuilder temp = new StringBuilder("(");
            foreach (T item in items)
            {
                Type t = item.GetType();
                PropertyInfo id = t.GetProperty("ID");
                object list = id.GetValue(item);
                temp.Append($"'{list}',");
            }
            temp.Remove(temp.Length - 1, 1);
            temp.Append(")");
            return temp.ToString();
        }
    }
}
