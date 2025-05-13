using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API_Structure.Core.Helpers
{
    public static class PhoneHelper
    {
        public static bool IsValidPhone(string phone)
        {
            var pattern = @"^01[0125]\d{8}$";

            return Regex.IsMatch(phone, pattern);
        }
    }
}
