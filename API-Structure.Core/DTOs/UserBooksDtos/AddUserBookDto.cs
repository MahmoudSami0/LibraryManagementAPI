using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.UserBooksDtos
{
    public class AddUserBookDto
    {
        public Guid userId { get; set; }
        public Guid bookId { get; set; }
        public DateOnly reservedTo { get; set; }
    }
}
