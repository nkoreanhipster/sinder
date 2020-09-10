using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder.Helpers
{
    public class PasswordHelper
    {
        public static(byte[] passwordhash, byte[] salt) GetPassword(string password)
        {
            byte[] passwordHash, Salt;

            CreatePasswordHash(password, out passwordHash, out Salt);

            return (passwordHash, Salt);

        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] salt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                salt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }
    }
}
