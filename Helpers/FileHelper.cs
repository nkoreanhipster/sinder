using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinder
{
    public static class FileHelper
    {
        public static byte[] ConvertIFormFileToByteArray(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string str = Convert.ToBase64String(fileBytes);
                byte[] bytes = Encoding.ASCII.GetBytes(str);
                return bytes;
            }
        }

        public static string ConvertIFormFileToString(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                string str = Convert.ToBase64String(fileBytes);
                return str;
            }
        }

        public static string Upload(IFormFile file)
        {
            string filePath = null;
            try{
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "/uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                filePath = Path.Combine(uploadPath, fileName);

                using (var strem = File.Create(filePath))
                {
                    file.CopyTo(strem);
                }
            }
            catch
            {
                filePath = null;
            }
            return filePath;
        }
    }
}
