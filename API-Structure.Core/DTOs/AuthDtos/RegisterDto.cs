using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagementSystem.Core.DTOs.UserDtos
{
    public class RegisterDto
    {
        public string fName { get; set; }
        public string lName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? Phone { get; set; }
        public string Password { get; set; }
    }
}
