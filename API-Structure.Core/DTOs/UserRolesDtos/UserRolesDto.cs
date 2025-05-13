using BookManagementSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.UserRolesDtos
{
    public class UserRolesDto
    {
        public Guid userId { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
    }
}
