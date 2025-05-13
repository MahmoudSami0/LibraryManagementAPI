using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.UserDtos
{
    public class AddUserDto : UserDto
    {
        public IFormFile? photo { get; set; }
    }
}
