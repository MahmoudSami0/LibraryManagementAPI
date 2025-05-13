using API_Structure.Core.DTOs.UserRolesDtos;
using API_Structure.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.RoleDtos
{
    public class ReturnedRoleDto
    {
        public Guid roleId { get; set; }
        public string roleName { get; set; }
        public List<UserRolesDto>? UsersInRole { get; set; }
    }
}
