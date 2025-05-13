using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.AuthDtos
{
    public class AddUserRoleDto
    {
        public Guid UserId { get; set; }
        public string Role { get; set; }
    }
}
