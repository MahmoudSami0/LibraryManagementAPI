using API_Structure.Core.Consts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace API_Structure.Core.Helpers
{
    public class PictuteHelper
    {
        public const string allowedExtensions = ".png, .jpg, .jpeg";
        public const long allowedSize = 12582912;


        public static bool IsAllowedExtension(IFormFile file)
        {
            return allowedExtensions.Contains(Path.GetExtension((file.FileName).ToLower()));
        }

        public static bool IsAllowedSize(IFormFile file)
        {
            return file.Length <= allowedSize;
        }
        public async static Task<MemoryStream> Upload(IFormFile data)
        {
            using var dataStream = new MemoryStream();
            await data.CopyToAsync(dataStream);
            
            return dataStream;
        }
    }
}
