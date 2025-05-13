using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.DTOs.UserBooksDtos
{
    public class UserBooksDto
    {
        public Guid bookId { get; set; }
        public string bookTitle { get; set; }
        public string bookDescription { get; set; }
        public DateOnly ReservedOn { get; set; }
        public DateOnly ReservedTo { get; set; }
        public byte[]? cover { get; set; }
    }
}
