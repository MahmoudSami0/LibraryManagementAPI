using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.UserDtos
{
    public class UserDto
    {
        [StringLength(50)]
        public string fName { get; set; }

        [StringLength(256)]
        public string lName { get; set; }

        [StringLength(150)]
        public string email { get; set; }

        [StringLength(50)]
        public string userName { get; set; }

        [StringLength(20)]
        public string? phone { get; set; }
    }
}
